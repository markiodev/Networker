using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Server
{
    //This class needs a lot of cleaning up, but I don't have the time to do it right now.
    public abstract class NetworkerServerBase : INetworkerServer
    {
        const int opsToPreAlloc = 2;

        private readonly ServerConfiguration configuration;
        protected readonly IContainerIoc container;
        public readonly INetworkerLogger Logger;
        private readonly PacketDeserializer packetDeserializer;
        private readonly Dictionary<string, Type> packetHandlers;
        private readonly Dictionary<string, IServerPacketHandler> packetHandlerSingletons;
        private readonly PacketSerializer packetSerializer;
        BufferManager _bufferManager;
        private bool _isRunning = true;
        Semaphore _maxNumberAcceptedClients;
        private SocketAsyncEventArgsPool _readWritePoolTcp;
        private SocketAsyncEventArgsPool _readWritePoolUdp;
        private Socket _tcpSocket;

        protected NetworkerServerBase(ServerConfiguration configuration,
            INetworkerLogger logger,
            IList<INetworkerPacketHandlerModule> modules,
            IContainerIoc container)
        {
            this.configuration = configuration;
            this.Logger = logger;
            this.container = container;
            this.Connections = new List<TcpConnection>();
            this.packetSerializer = new PacketSerializer();
            this.packetDeserializer = new PacketDeserializer();
            this.packetHandlers = new Dictionary<string, Type>();
            this.packetHandlerSingletons = new Dictionary<string, IServerPacketHandler>();

            foreach(var module in modules)
            {
                this.RegisterTypesFromModule(module);
            }

            this.container.RegisterSingleton<ITcpConnectionsProvider>(
                new TcpConnectionsProvider(this.Connections));
            this.container.RegisterSingleton(logger);

            this.RegisterPacketHandler<PingRequestPacket, PingRequestPacketHandler>();
        }

        public List<TcpConnection> Connections { get; }

        public Socket _udpListener { get; set; }

        public EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        public EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }

        public void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }

        public void Broadcast<T>(T packet)
            where T: NetworkerPacketBase
        {
            if(!this.configuration.UseUdp)
            {
                throw new Exception("Cannot broadcast packet when UDP is not enabled.");
            }

            this._udpListener.SendTo(this.packetSerializer.Serialize(packet),
                new IPEndPoint(IPAddress.Parse(this.configuration.IpAddresses[0]),
                    this.configuration.UdpPortRemote));
        }

        public IContainerIoc GetIocContainer()
        {
            return this.container;
        }

        public INetworkerServer RegisterPacketHandler<TPacketType, TPacketHandlerType>()
            where TPacketHandlerType: IServerPacketHandler
        {
            var handler = typeof(TPacketHandlerType);
            this.packetHandlers.Add(typeof(TPacketType).Name, handler);
            this.container.RegisterType(handler);

            return this;
        }

        public INetworkerServer RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            var module =
                (INetworkerPacketHandlerModule)Activator.CreateInstance(typeof(TPacketHandlerModule));
            this.RegisterTypesFromModule(module);
            return this;
        }

        public INetworkerServer Start()
        {
            if(this.configuration.UseTcp)
            {
                this.Logger.Trace($"Starting TCP Listener on port {this.configuration.TcpPort}.");

                this._bufferManager = new BufferManager(
                    this.configuration.Advanced.PacketBufferSize
                    * (this.configuration.Advanced.MaxTcpConnections
                       + this.configuration.Advanced.UdpSocketPoolSize) * opsToPreAlloc,
                    this.configuration.Advanced.PacketBufferSize);
                this._bufferManager.InitBuffer();
                this._maxNumberAcceptedClients = new Semaphore(this.configuration.Advanced.MaxTcpConnections,
                    this.configuration.Advanced.MaxTcpConnections);

                this._readWritePoolTcp =
                    new SocketAsyncEventArgsPool(this.configuration.Advanced.MaxTcpConnections);
                this._readWritePoolUdp =
                    new SocketAsyncEventArgsPool(this.configuration.Advanced.UdpSocketPoolSize);

                SocketAsyncEventArgs readWriteEventArg;

                for(int i = 0; i < this.configuration.Advanced.MaxTcpConnections; i++)
                {
                    readWriteEventArg = new SocketAsyncEventArgs();
                    readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(this.IO_Completed);
                    readWriteEventArg.UserToken = new AsyncUserToken();

                    this._bufferManager.SetBuffer(readWriteEventArg);

                    this._readWritePoolTcp.Push(readWriteEventArg);
                }

                for(int i = 0; i < this.configuration.Advanced.UdpSocketPoolSize; i++)
                {
                    readWriteEventArg = new SocketAsyncEventArgs();
                    readWriteEventArg.Completed += this.ProcessReceiveUdp;
                    readWriteEventArg.RemoteEndPoint =
                        new IPEndPoint(IPAddress.Any, this.configuration.UdpPortLocal);

                    this._bufferManager.SetBuffer(readWriteEventArg);

                    this._readWritePoolUdp.Push(readWriteEventArg);
                }

                this._tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                this._tcpSocket.Bind(new IPEndPoint(IPAddress.Parse(this.configuration.IpAddresses[0]),
                    this.configuration.TcpPort));

                this._tcpSocket.Listen(this.configuration.Advanced.MaxTcpConnections);

                new Thread(new ThreadStart(() => { this.StartAccept(null); })).Start();
            }

            if(this.configuration.UseUdp)
            {
                this.Logger.Trace($"Starting UDP Listener on port {this.configuration.UdpPortLocal}.");
                this.Logger.Trace($"Starting UDP Sender on port {this.configuration.UdpPortRemote}.");

                this._udpListener =
                    new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this._udpListener.Bind(new IPEndPoint(IPAddress.Any, this.configuration.UdpPortLocal));

                this.ProcessUdp();
            }

            return this;
        }

        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if(acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed +=
                    new EventHandler<SocketAsyncEventArgs>(this.AcceptEventArg_Completed);
            }
            else
            {
                acceptEventArg.AcceptSocket = null;
            }

            this._maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = this._tcpSocket.AcceptAsync(acceptEventArg);
            if(!willRaiseEvent)
            {
                this.ProcessAccept(acceptEventArg);
            }
        }

        public void Stop()
        {
            this._isRunning = false;
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            this.Logger.Trace(
                $"TCP Client Disconnected. IP: {(token.Socket.RemoteEndPoint as IPEndPoint).Address}");

            var connection =
                this.Connections.FirstOrDefault(f => f.Socket == ((AsyncUserToken)e.UserToken).Socket);

            if(connection != null)
            {
                this.Connections.Remove(connection);
                this.ClientDisconnected?.Invoke(this, new TcpConnectionDisconnectedEventArgs(connection));
            }

            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            catch(Exception) { }
            
            this._readWritePoolTcp.Push(e);
            this._maxNumberAcceptedClients.Release();
        }

        private IServerPacketHandler GetPacketHandler(Type packetHandlerType)
        {
            if(this.configuration.Advanced.RegisterPacketHandlersAsSingletons)
            {
                if(!this.packetHandlerSingletons.ContainsKey(packetHandlerType.Name))
                {
                    lock(this.packetHandlerSingletons)
                    {
                        if(!this.packetHandlerSingletons.ContainsKey(packetHandlerType.Name))
                        {
                            var resolvedPacketHandler =
                                this.container.Resolve<IServerPacketHandler>(packetHandlerType);

                            this.packetHandlerSingletons.Add(packetHandlerType.Name, resolvedPacketHandler);
                        }
                    }
                }

                return this.packetHandlerSingletons[packetHandlerType.Name];
            }
            else
            {
                return this.container.Resolve<IServerPacketHandler>(packetHandlerType);
            }
        }

        private void HandlePacket(INetworkerConnection connection,
            NetworkerPacketBase deserialized,
            byte[] bytes)
        {
            if(!this.packetHandlers.ContainsKey(deserialized.UniqueKey))
            {
                this.Logger.Error(
                    new Exception($"No packet handler could be loaded for {deserialized.UniqueKey}"));
                return;
            }

            var packetHandlerType = this.packetHandlers[deserialized.UniqueKey];
            var packetHandler = this.GetPacketHandler(packetHandlerType);
            packetHandler.Handle(connection, deserialized, bytes);
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch(e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException(
                        "The last operation completed on the socket was not a receive or send");
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs readEventArgs = this._readWritePoolTcp.Pop();

            ((AsyncUserToken)readEventArgs.UserToken).Socket = e.AcceptSocket;

            var connection = new TcpConnection(((AsyncUserToken)readEventArgs.UserToken).Socket);

            this.Logger.Trace(
                $"TCP Client Connected. IP: {(connection.Socket.RemoteEndPoint as IPEndPoint).Address}");

            this.Connections.Add(connection);
            this.ClientConnected?.Invoke(this, new TcpConnectionConnectedEventArgs(connection));

            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
            if(!willRaiseEvent)
            {
                this.ProcessReceive(readEventArgs);
            }

            this.StartAccept(e);
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if(e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                e.SetBuffer(e.Offset, e.BytesTransferred);

                var packets = this.packetDeserializer.GetPacketsFromSocket(e);

                foreach(var packet in packets)
                {
                    var connection =
                        this.Connections.FirstOrDefault(
                            f => f.Socket == ((AsyncUserToken)e.UserToken).Socket);
                    this.HandlePacket(connection, packet.Item1, packet.Item2);
                }

                this.ProcessSend(e);
            }
            else
            {
                this.CloseClientSocket(e);
            }
        }

        private void ProcessReceiveUdp(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessUdp();

            if(e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                e.SetBuffer(e.Offset, e.BytesTransferred);
                var packets = this.packetDeserializer.GetPacketsFromSocket(e);

                foreach(var packet in packets)
                {
                    this.HandlePacket(new UdpConnection(this._udpListener), packet.Item1, packet.Item2);
                }
            }

            this._readWritePoolUdp.Push(e);
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if(e.SocketError == SocketError.Success)
            {
                AsyncUserToken token = (AsyncUserToken)e.UserToken;

                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if(!willRaiseEvent)
                {
                    this.ProcessReceive(e);
                }
            }
            else
            {
                this.CloseClientSocket(e);
            }
        }

        private void ProcessUdp()
        {
            var recieveArgs = this._readWritePoolUdp.Pop();
            this._bufferManager.SetBuffer(recieveArgs);

            if(!this._udpListener.ReceiveFromAsync(recieveArgs))
            {
                this.ProcessReceiveUdp(this, recieveArgs);
            }
        }

        private void RegisterTypesFromModule(INetworkerPacketHandlerModule packetHandlerModule)
        {
            foreach(var packetHandler in packetHandlerModule.RegisterPacketHandlers())
            {
                this.packetHandlers.Add(packetHandler.Key.Name, packetHandler.Value);

                this.container.RegisterType(packetHandler.Value);
            }
        }
    }
}