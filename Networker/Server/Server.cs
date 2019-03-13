using Networker.Common.Abstractions;
using Networker.Server.Abstractions;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Networker.Server
{
    public class Server : IServer
    {
        public ITcpSocketListener TcpListener { get; }
        public IUdpSocketListener UdpListener { get; }
        public IServerInformation Information { get; }

        public EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        public EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }
        public EventHandler<ServerInformationEventArgs> ServerInformationUpdated { get; set; }

        private readonly IPacketSerialiser packetSerialiser;
        private readonly ServerBuilderOptions options;
        private readonly ITcpConnections tcpConnections;
        private ServerInformationEventArgs eventArgs;

        public Server(ServerBuilderOptions options,
            ITcpConnections tcpConnections,
            ITcpSocketListenerFactory tcpSocketListenerFactory,
            IUdpSocketListener udpSocketListener,
            IBufferManager bufferManager,
            IServerInformation serverInformation,
            IPacketSerialiser packetSerialiser)
        {
            this.options = options;
            this.tcpConnections = tcpConnections;
            Information = serverInformation;
            this.packetSerialiser = packetSerialiser;
            bufferManager.InitBuffer();

            if (options.TcpPort > 0) TcpListener = tcpSocketListenerFactory.Create();

            if (options.UdpPort > 0) UdpListener = udpSocketListener;

            Task.Factory.StartNew(() =>
            {
                while (Information.IsRunning)
                {
                    if (eventArgs == null) eventArgs = new ServerInformationEventArgs();

                    eventArgs.ProcessedTcpPackets = serverInformation.ProcessedTcpPackets;
                    eventArgs.InvalidTcpPackets = serverInformation.InvalidTcpPackets;
                    eventArgs.ProcessedUdpPackets = serverInformation.ProcessedUdpPackets;
                    eventArgs.InvalidUdpPackets = serverInformation.InvalidUdpPackets;
                    eventArgs.TcpConnections = tcpConnections.GetConnections().Count;

                    ServerInformationUpdated?.Invoke(this, eventArgs);

                    Information.ProcessedTcpPackets = 0;
                    Information.InvalidTcpPackets = 0;
                    Information.ProcessedUdpPackets = 0;
                    Information.InvalidUdpPackets = 0;

                    Thread.Sleep(10000);
                }
            });
        }

        public void Broadcast<T>(T packet) => Broadcast(packetSerialiser.Serialise(packet));
        public void Broadcast(byte[] packet)
        {
            if (UdpListener == null) throw new Exception("UDP is not enabled");

            var socket = UdpListener.GetSocket();
            socket.EnableBroadcast = true;
            socket.SendTo(packet, new IPEndPoint(IPAddress.Broadcast, this.options.UdpPort));
        }

        public ITcpConnections GetConnections()
        {
            return tcpConnections;
        }

        public void Start()
        {
            Information.IsRunning = true;

            TcpListener?.Listen();
            UdpListener?.Listen();

            if (TcpListener != null)
            {
                TcpListener.ClientConnected += ClientConnectedEvent;
                TcpListener.ClientDisconnected += ClientDisconnectedEvent;
            }
        }

        public void Stop()
        {
            Information.IsRunning = false;

            if (TcpListener != null)
            {
                TcpListener.ClientConnected -= ClientConnectedEvent;
                TcpListener.ClientDisconnected -= ClientDisconnectedEvent;
            }
        }

        private void ClientConnectedEvent(object sender, TcpConnectionConnectedEventArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }

        private void ClientDisconnectedEvent(object sender, TcpConnectionDisconnectedEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }
    }
}
