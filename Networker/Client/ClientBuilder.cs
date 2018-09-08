using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Networker.Client.Abstractions;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Client
{
    public class ClientBuilder : BuilderBase<IClientBuilder, IClient, ClientBuilderOptions>, IClientBuilder
    {
        public ClientBuilder() : base()
        {

        }

        public override IClient Build()
        {
            SetupSharedDependencies();
            this.serviceCollection.AddSingleton<IClient, Client>();
            this.serviceCollection.AddSingleton<IClientPacketProcessor, ClientPacketProcessor>();

            var serviceProvider = GetServiceProvider();

            return serviceProvider.GetService<IClient>();
        }

        public IClientBuilder SetPacketBufferPoolSize(int size)
        {
            this.options.ObjectPoolSize = size;
            return this;
        }

        public IClientBuilder UseIp(string ip)
        {
            this.options.Ip = ip;
            return this;
        }

        public IClientBuilder UseUdp(int port, int localPort)
        {
            this.options.UdpPort = port;
            this.options.UdpPortLocal = localPort;
            return this;
        }
    }
}