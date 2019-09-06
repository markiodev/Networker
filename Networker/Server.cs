using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Networker.Events;

namespace Networker
{
    public class Server : IServer
    {
        private Socket _tcpSocket;
        private int _tcpPort;
        private int _udpPort;
        private int _maxConnections;
        private int _bufferLength = 10000;
        private Dictionary<int, IConnection> _connections;
        private SocketEventArgsObjectPool _socketEventArgsObjectPool;
        private PacketProcessor _packetProcessor;
        private Action<Exception> _onErrorAction;
        private PacketContextObjectPool _packetContextObjectPool;
        private Dictionary<int, IPacketHandler> _packetHandlers;
        private Action<ClientConnectedEvent> _onClientConnected;
        private Action<ClientDisconnectedEvent> _onClientDisconnected;
        private Action<string, string> _logAction;

        internal void SetOnErrorAction(Action<Exception> onErrorAction)
        {
            _onErrorAction = onErrorAction;
        }

        internal void SetTcpPort(int port)
        {
            _tcpPort = port;
        }

        internal void SetMaxConnections(int maxConnections)
        {
            _maxConnections = maxConnections;
        }

        internal void SetOnClientDisconnected(Action<ClientDisconnectedEvent> action)
        {
            _onClientDisconnected = action;
        }

        internal void SetOnClientConnected(Action<ClientConnectedEvent> action)
        {
            _onClientConnected = action;
        }

        public Server()
        {
            _packetHandlers = new Dictionary<int, IPacketHandler>();
        }

        public void Start()
        {
            _connections = new Dictionary<int, IConnection>(_maxConnections);
            _socketEventArgsObjectPool = new SocketEventArgsObjectPool(_maxConnections);
            _packetContextObjectPool = new PacketContextObjectPool(_maxConnections * 3);
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpSocket.Bind(new IPEndPoint(IPAddress.Any, _tcpPort));
            _packetProcessor = new PacketProcessor(_packetHandlers, _packetContextObjectPool);

            for (int i = 0; i < _maxConnections; i++)
            {
                var connection = new TcpConnection();
                connection.Id = i;

                var socketTokenEventArgs = new SocketAsyncEventArgs();
                socketTokenEventArgs.Completed += Completed;
                socketTokenEventArgs.UserToken = new SocketToken
                {
                    Id = i,
                    Connection = connection
                };

                connection.SocketToken = socketTokenEventArgs.UserToken as SocketToken;

                socketTokenEventArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(_bufferLength), 0, _bufferLength);
                _socketEventArgsObjectPool.Push(socketTokenEventArgs);
            }

            for (var i = 0; i < _packetContextObjectPool.Capacity; i++)
            {
                _packetContextObjectPool.Push(new PacketContext());
            }
            
            _tcpSocket.Listen(_maxConnections);
            
            WriteLog("Debug", "Server started");
            State = ServerState.Running;

            StartAccept(null);
        }

        //Completed()
        private void Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e, null);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                default:
                    return;
                    /*throw new ArgumentException(
                        "The last operation completed on the socket was not a receive or send")*/
            }
        }

        //ProcessReceive
        private void ProcessReceive(SocketAsyncEventArgs e, SocketToken token)
        {
            try
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    if (token == null)
                    {
                        token = (SocketToken)e.UserToken;
                    }

                    unsafe
                    {
                        WriteLog("Debug", "New TCP Packet from ID " + token.Id);

                        var packetContext = _packetContextObjectPool.Pop();

                        packetContext.Packet = Marshal.AllocHGlobal(e.BytesTransferred);
                        var packetBufferSpan = new Span<byte>(e.Buffer, e.Offset, e.BytesTransferred);
                        var packetSpan = new Span<byte>(packetContext.Packet.ToPointer(), e.BytesTransferred);

                        packetBufferSpan.CopyTo(packetSpan);

                        packetContext.ConnectionId = token.Id;

                        _packetProcessor.Process(packetContext);
                    }

                    this.ProcessSend(e);
                }
                else
                {
                    this.CloseSocketClient(e);
                }
            }
            catch (Exception exception)
            {
                _onErrorAction.Invoke(exception);
            }
        }

        //ProcessSend
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                SocketToken token = (SocketToken)e.UserToken;
               
                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    this.ProcessReceive(e, token);
                }
            }
            else
            {
                this.CloseSocketClient(e);
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs readEventArgs = _socketEventArgsObjectPool.Pop();
            var socketToken = (SocketToken)readEventArgs.UserToken;

            socketToken.Socket = e.AcceptSocket;

            //Event it has connected
            _onClientConnected?.Invoke(new ClientConnectedEvent { Connection = socketToken.Connection });
            
            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
            {
                this.ProcessReceive(readEventArgs, (SocketToken)readEventArgs.UserToken);
            }

            this.StartAccept(e);
        }

        //StartAccept
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            try
            {
                if (acceptEventArg == null)
                {
                    acceptEventArg = _socketEventArgsObjectPool.Pop();
                    acceptEventArg.Completed += EventArgCompleted;
                }
                else
                {
                    acceptEventArg.AcceptSocket = null;
                }
                                
                bool willRaiseEvent = _tcpSocket.AcceptAsync(acceptEventArg);
                if (!willRaiseEvent)
                {
                    this.ProcessAccept(acceptEventArg);
                }
            }
            catch (Exception e)
            {
                _onErrorAction.Invoke(e);
            }
        }

        public void EventArgCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }

        //CloseSocketClient
        private void CloseSocketClient(SocketAsyncEventArgs e)
        {
            SocketToken token = e.UserToken as SocketToken;

            _onClientDisconnected?.Invoke(new ClientDisconnectedEvent
            {
                Id = token.Id
            });

            try
            {
                token?.Socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception ex)
            {
                _onErrorAction.Invoke(ex);
            }

            _socketEventArgsObjectPool.Push(e);
        }

        public void Stop()
        {
        }

        public void RegisterModule(IModule module)
        {
            foreach (var packetHandler in module.PacketHandlers)
            {
                _packetHandlers.Add(packetHandler.Key, packetHandler.Value);
            }
        }

        public int ActiveConnections => _connections.Count;
        public ServerState State { get; set; }

        public void Broadcast(byte[] packet)
        {
        }

        public void Broadcast<T>(int packetType, T packet)
        {
        }

        public void WriteLog(string logType, string message)
        {
            _logAction.Invoke(logType, message);
        }

        public IConnection GetConnection(int connectionId)
        {
            return _connections[connectionId];
        }

        public void SetUdpPort(int port)
        {
            _udpPort = port;
        }

        public void SetOnLogAction(Action<string, string> action)
        {
            _logAction = action;
        }
    }
}