using System;
using System.Threading;
using System.Threading.Tasks;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class Server : IServer
    {
        private readonly IPacketSerialiser packetSerialiser;
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

                    eventArgs.ProcessedTcpPackets =
                        serverInformation.ProcessedTcpPackets;
                    eventArgs.InvalidTcpPackets =
                        serverInformation.InvalidTcpPackets;
                    eventArgs.ProcessedUdpPackets =
                        serverInformation.ProcessedUdpPackets;
                    eventArgs.InvalidUdpPackets =
                        serverInformation.InvalidUdpPackets;
                    eventArgs.TcpConnections = tcpConnections.GetConnections()
                        .Count;

                    ServerInformationUpdated?.Invoke(this, eventArgs);

                    Information.ProcessedTcpPackets = 0;
                    Information.InvalidTcpPackets = 0;
                    Information.ProcessedUdpPackets = 0;
                    Information.InvalidUdpPackets = 0;

                    Thread.Sleep(10000);
                }
            });
        }

        public ITcpSocketListener TcpListener { get; }
        public IUdpSocketListener UdpListener { get; }
        public IServerInformation Information { get; }
        public EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        public EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }
        public EventHandler<ServerInformationEventArgs> ServerInformationUpdated { get; set; }

        public void Broadcast<T>(T packet)
        {
            if (UdpListener == null) throw new Exception("UDP is not enabled");

            var socket = UdpListener.GetSocket();
            var endpoint = UdpListener.GetEndPoint();
            socket.SendTo(packetSerialiser.Serialise(packet), endpoint);
        }

        public ITcpConnections GetConnections()
        {
            return tcpConnections;
        }

        public void Start()
        {
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