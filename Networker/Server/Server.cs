using System;
using System.Threading;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class Server : IServer
    {
        private readonly IPacketSerialiser packetSerialiser;
        private readonly IServerInformation serverInformation;
        private readonly ITcpConnections tcpConnections;
        private ServerInformationEventArgs eventArgs;

        public Server(ServerBuilderOptions options,
            ILogger logger,
            IServiceProvider serviceProvider,
            IPacketHandlers packetHandlers,
            ITcpConnections tcpConnections,
            IServerPacketProcessor serverPacketProcessor,
            ITcpSocketListenerFactory tcpSocketListenerFactory,
            IUdpSocketListenerFactory udpSocketListenerFactory,
            IBufferManager bufferManager,
            IServerInformation serverInformation,
            IPacketSerialiser packetSerialiser)
        {
            this.tcpConnections = tcpConnections;
            this.serverInformation = serverInformation;
            this.packetSerialiser = packetSerialiser;
            bufferManager.InitBuffer();

            if(options.TcpPort > 0)
            {
                this.TcpListener = tcpSocketListenerFactory.Create();
            }

            if(options.UdpPort > 0)
            {
                this.UdpListener = udpSocketListenerFactory.Create();
            }

            Task.Factory.StartNew(() =>
                                  {
                                      while(this.serverInformation.IsRunning)
                                      {
                                          if(this.eventArgs == null)
                                          {
                                              this.eventArgs = new ServerInformationEventArgs();
                                          }

                                          this.eventArgs.ProcessedTcpPackets =
                                              serverInformation.ProcessedTcpPackets;
                                          this.eventArgs.InvalidTcpPackets =
                                              serverInformation.InvalidTcpPackets;
                                          this.eventArgs.ProcessedUdpPackets =
                                              serverInformation.ProcessedUdpPackets;
                                          this.eventArgs.InvalidUdpPackets =
                                              serverInformation.InvalidUdpPackets;

                                          this.ServerInformationUpdated?.Invoke(this, this.eventArgs);

                                          this.serverInformation.ProcessedTcpPackets = 0;
                                          this.serverInformation.InvalidTcpPackets = 0;
                                          this.serverInformation.ProcessedUdpPackets = 0;
                                          this.serverInformation.InvalidUdpPackets = 0;

                                          Thread.Sleep(10000);
                                      }
                                  });
        }

        public ITcpSocketListener TcpListener { get; }
        public IUdpSocketListener UdpListener { get; }
        public EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        public EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }
        public EventHandler<ServerInformationEventArgs> ServerInformationUpdated { get; set; }

        public void Broadcast(PacketBase packet)
        {
            if(this.UdpListener == null)
            {
                throw new Exception("UDP is not enabled");
            }

            var socket = this.UdpListener.GetSocket();
            var endpoint = this.UdpListener.GetEndPoint();
            socket.SendTo(this.packetSerialiser.Serialise(packet), endpoint);
        }

        public ITcpConnections GetConnections()
        {
            return this.tcpConnections;
        }

        public void Start()
        {
            this.TcpListener?.Listen();
            this.UdpListener?.Listen();

            if(this.TcpListener != null)
            {
                this.TcpListener.ClientConnected += this.ClientConnectedEvent;
                this.TcpListener.ClientDisconnected += this.ClientDisconnectedEvent;
            }
        }

        public void Stop()
        {
            this.serverInformation.IsRunning = false;

            if(this.TcpListener != null)
            {
                this.TcpListener.ClientConnected -= this.ClientConnectedEvent;
                this.TcpListener.ClientDisconnected -= this.ClientDisconnectedEvent;
            }
        }

        private void ClientConnectedEvent(object sender, TcpConnectionConnectedEventArgs e)
        {
            this.ClientConnected?.Invoke(this, e);
        }

        private void ClientDisconnectedEvent(object sender, TcpConnectionDisconnectedEventArgs e)
        {
            this.ClientDisconnected?.Invoke(this, e);
        }
    }
}