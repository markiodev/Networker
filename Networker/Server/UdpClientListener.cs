using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class UdpClientListener : IUdpSocketListener
    {
        private readonly ObjectPool<EndPoint> endPointPool;
        private readonly ILogger logger;
        private readonly ServerBuilderOptions options;
        private readonly IServerPacketProcessor serverPacketProcessor;
        private UdpClient client;
        private IPEndPoint endPoint;
        private readonly IServerInformation serverInformation;

        public UdpClientListener(ServerBuilderOptions options,
            ILogger<UdpClientListener> logger,
            IServerPacketProcessor serverPacketProcessor,
            IBufferManager bufferManager,
            IServerInformation serverInformation)
        {
            this.options = options;
            this.logger = logger;
            this.serverPacketProcessor = serverPacketProcessor;
            this.serverInformation = serverInformation;

            this.endPointPool = new ObjectPool<EndPoint>(this.options.UdpSocketObjectPoolSize);

            for(var i = 0; i < this.endPointPool.Capacity; i++)
            {
                this.endPointPool.Push(new IPEndPoint(IPAddress.Loopback, this.options.UdpPort));
            }
        }

        public IPEndPoint GetEndPoint()
        {
            return this.endPoint;
        }

        public Socket GetSocket()
        {
            return this.client.Client;
        }

        public void Listen()
        {
            this.client = new UdpClient(this.options.UdpPort);
            this.endPoint = new IPEndPoint(IPAddress.Loopback, this.options.UdpPort);

            this.logger.LogInformation($"Starting UDP listener on port {this.options.UdpPort}.");

            Task.Factory.StartNew(() =>
                                  {
                                      while(this.serverInformation.IsRunning)
                                      {
                                          var endpoint = this.endPointPool.Pop() as IPEndPoint;

                                          var receivedResults = this.client.Receive(ref endpoint);

                                          this.Process(endpoint, receivedResults);
                                      }
                                  });
        }

        private async Task Process(EndPoint endPoint, byte[] buffer)
        {
            this.serverPacketProcessor.ProcessUdpFromBuffer(endPoint, buffer);

            this.endPointPool.Push(endPoint);
        }
    }
}