using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Networker
{
    public class Client : IClient
    {
        private readonly PacketContextObjectPool _packetContextObjectPool;
        private readonly Dictionary<int, IPacketHandler> _packetHandlers;
        private readonly PingOptions _pingOptions;
        private readonly byte[] _pingPacketBuffer;
        private string _ip;
        private PacketProcessor _packetProcessor;
        private int _tcpPort;
        private Socket _tcpSocket;

        public Client()
        {
            _pingPacketBuffer = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString());
            _pingOptions = new PingOptions(64, true);
            _packetHandlers = new Dictionary<int, IPacketHandler>();
            _packetContextObjectPool = new PacketContextObjectPool(1000);
        }

        public ConnectResult Connect()
        {
            try
            {
                if (_packetProcessor == null)
                {
                    for (var i = 0; i < _packetContextObjectPool.Capacity; i++)
                    {
                        _packetContextObjectPool.Push(new PacketContext
                        {
                            PacketBytes = new byte[10000]
                        });
                    }
                    _packetProcessor = new PacketProcessor(_packetHandlers, _packetContextObjectPool);
                }

                _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _tcpSocket.Connect(_ip, _tcpPort);
            }
            catch (Exception exception)
            {
                return new ConnectResult(exception.Message);
            }

            StartTcpSocketThread();

            return new ConnectResult();
        }

        public long Ping(int timeout)
        {
            var pingSender = new Ping();
            var reply = pingSender.Send(_ip, timeout, _pingPacketBuffer, _pingOptions);

            if (reply.Status == IPStatus.Success) return reply.RoundtripTime;

            return -1;
        }

        public ConnectionState ConnectionState =>
            _tcpSocket.Connected ? ConnectionState.Connected : ConnectionState.Disconnected;

        public void Send(byte[] packet)
        {
            _tcpSocket.Send(packet);
        }

        public void Send(int packetIdentifier, byte[] packet)
        {
            unsafe
            {
                var newEventBytes = Marshal.AllocHGlobal(packet.Length + 4);
                var newEventBytesSpan = new Span<byte>(newEventBytes.ToPointer(), packet.Length + 4);
                BitConverter.GetBytes(packetIdentifier).CopyTo(newEventBytesSpan.Slice(0, 4));
                packet.CopyTo(newEventBytesSpan.Slice(4, packet.Length));

                _tcpSocket.Send(newEventBytesSpan.ToArray());

                Marshal.FreeHGlobal(newEventBytes);
            }
        }

        public void RegisterModule(IModule module)
        {
            foreach (var packetHandler in module.PacketHandlers)
            {
                _packetHandlers.Add(packetHandler.Key, packetHandler.Value);
            }
        }

        internal void SetIp(string ip)
        {
            _ip = ip;
        }

        internal void SetTcpPort(int tcpPort)
        {
            _tcpPort = tcpPort;
        }

        private void StartTcpSocketThread()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_tcpSocket.Poll(10, SelectMode.SelectWrite))
                    {
                        var context = _packetContextObjectPool.Pop();

                        _tcpSocket.Receive(context.PacketBytes);

                        _packetProcessor.Process(context);
                    }

                    if (!_tcpSocket.Connected)
                        //this.Disconnected?.Invoke(this, this.tcpSocket);
                        break;
                }

                if (_tcpSocket.Connected)
                {
                    _tcpSocket.Disconnect(false);
                    _tcpSocket.Close();
                    //this.Disconnected?.Invoke(this, this.tcpSocket);
                }

                _tcpSocket = null;
            });
        }
    }
}