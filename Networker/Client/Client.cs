using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Client.Abstractions;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Client
{
    public class Client : IClient
    {
        private readonly ILogger<Client> logger;
        private readonly ClientBuilderOptions options;
        private readonly IClientPacketProcessor packetProcessor;
        private readonly IPacketSerialiser packetSerialiser;
        private readonly byte[] pingPacketBuffer;
        private bool isRunning = true;
        private readonly PingOptions pingOptions;
        private Socket tcpSocket;
        private UdpClient udpClient;
        private IPEndPoint udpEndpoint;

        public Client(ClientBuilderOptions options,
            IPacketSerialiser packetSerialiser,
            IClientPacketProcessor packetProcessor,
            ILogger<Client> logger)
        {
            this.options = options;
            this.packetSerialiser = packetSerialiser;
            this.packetProcessor = packetProcessor;
            this.logger = logger;
            this.pingPacketBuffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaa");
            this.pingOptions = new PingOptions(64, true);
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
                this.udpClient = new UdpClient();
                this.udpClient.ExclusiveAddressUse = false;
                this.udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                var address = IPAddress.Parse(this.options.Ip);
                this.udpEndpoint = new IPEndPoint(address, this.options.UdpPort);
				this.udpClient.Client.Bind(this.udpEndpoint);

                Task.Factory.StartNew(() =>
                                      {
                                          this.logger.LogInformation(
                                              $"Connecting to UDP at {this.options.Ip}:{this.options.UdpPort}");

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

        public long Ping()
        {
            var pingSender = new Ping();
            var timeout = 10000;
            var reply = pingSender.Send(this.options.Ip, timeout, this.pingPacketBuffer, this.pingOptions);

            if(reply.Status == IPStatus.Success)
            {
                return reply.RoundtripTime;
            }

            this.logger.LogError($"Could not get ping " + reply.Status);
            return -1;
        }

        public void Send<T>(T packet)
        {
            if(this.tcpSocket == null)
            {
                throw new Exception("TCP client has not been initialised. Have you called .Connect()?");
            }

            var serialisedPacket = this.packetSerialiser.Serialise(packet);

            var result = this.tcpSocket.Send(serialisedPacket);
        }

        public void SendUdp(byte[] packet)
        {
            if(this.udpClient == null)
            {
                throw new Exception("UDP client has not been initialised. Have you called .Connect()?");
            }

            this.udpClient.Send(packet, packet.Length, this.udpEndpoint);
        }

        public void SendUdp<T>(T packet)
        {
            if(this.udpClient == null)
            {
                throw new Exception("UDP client has not been initialised. Have you called .Connect()?");
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