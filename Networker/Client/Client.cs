using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networker.Client.Abstractions;
using Networker.Common.Abstractions;

namespace Networker.Client
{
    public class Client : IClient
    {
        private readonly ClientBuilderOptions options;
        private readonly IClientPacketProcessor packetProcessor;
        private readonly ILogger logger;
        private readonly IPacketSerialiser packetSerialiser;
        private bool isRunning = true;
        private Socket tcpSocket;
        private UdpClient udpClient;
        private IPEndPoint udpEndpoint;

        public Client(ClientBuilderOptions options,
            IPacketSerialiser packetSerialiser,
            IClientPacketProcessor packetProcessor,
            ILogger logger)
        {
            this.options = options;
            this.packetSerialiser = packetSerialiser;
            this.packetProcessor = packetProcessor;
            this.logger = logger;
        }

        public EventHandler<Socket> Connected { get; set; }
        public EventHandler<Socket> Disconnected { get; set; }

        public void Connect()
        {
            if(this.options.TcpPort > 0 && this.tcpSocket == null)
            {
                this.tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.tcpSocket.Connect(this.options.Ip, this.options.TcpPort);
                this.Connected?.Invoke(this, this.tcpSocket);

                Task.Factory.StartNew(() =>
                                      {
                                          while(this.isRunning)
                                          {
                                              if(this.tcpSocket.Poll(10, SelectMode.SelectWrite))
                                              {
                                                  this.packetProcessor.Process(this.tcpSocket);
                                              }

                                              if(!this.tcpSocket.Connected)
                                              {
                                                  this.Disconnected?.Invoke(this, this.tcpSocket);
                                                  break;
                                              }
                                          }

                                          if(this.tcpSocket.Connected)
                                          {
                                              this.tcpSocket.Disconnect(false);
                                              this.tcpSocket.Close();
                                              this.Disconnected?.Invoke(this, this.tcpSocket);
                                          }

                                          this.tcpSocket = null;
                                      });
            }

            if(this.options.UdpPort > 0 && this.udpClient == null)
            {
                this.udpClient = new UdpClient(this.options.UdpPortLocal);
                var address = IPAddress.Parse(this.options.Ip);
                this.udpEndpoint = new IPEndPoint(address, this.options.UdpPort);

                Task.Factory.StartNew(() =>
                                      {
                                          this.logger.Info($"Connecting to UDP at {this.options.Ip}:{this.options.UdpPort}");

                                          while(this.isRunning)
                                          {
                                              try
                                              {
                                                  var data = this.udpClient.ReceiveAsync()
                                                                 .GetAwaiter()
                                                                 .GetResult();

                                                  this.packetProcessor.Process(data);
                                              }
                                              catch(Exception ex)
                                              {
                                                  this.logger.Error(ex);
                                              }
                                          }
                                          this.udpClient = null;
                                      });
            }
        }

        public int Ping()
        {
            throw new NotImplementedException();
            return 1235;
        }

        public void Send<T>(T packet)
        {
            if(this.tcpSocket == null)
            {
                throw new Exception("TCP client has not been initialised");
            }

            var serialisedPacket = this.packetSerialiser.Serialise(packet);

            var result = this.tcpSocket.Send(serialisedPacket);
        }

        public void SendUdp<T>(T packet)
        {
            if(this.udpClient == null)
            {
                throw new Exception("UDP client has not been initialised");
            }

            var serialisedPacket = this.packetSerialiser.Serialise(packet);

            this.udpClient.Send(serialisedPacket, serialisedPacket.Length, this.udpEndpoint);
        }

        public void Stop()
        {
            this.isRunning = false;
        }
    }
}