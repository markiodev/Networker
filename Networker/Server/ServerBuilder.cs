using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class ServerBuilder : BuilderBase<IServerBuilder, IServer, ServerBuilderOptions>, IServerBuilder
    {
        private Type tcpSocketListenerFactory;
        private Type udpSocketListenerFactory;

        public ServerBuilder() : base()
        {

        }

        public override IServer Build()
        {
            SetupSharedDependencies();

            this.serviceCollection.AddSingleton<ITcpConnections, TcpConnections>();
            this.serviceCollection.AddSingleton<IServer, Server>();
            this.serviceCollection.AddSingleton<IServerInformation, ServerInformation>();
            this.serviceCollection.AddSingleton<IServerPacketProcessor, ServerPacketProcessor>();
            this.serviceCollection.AddSingleton<IBufferManager>(new BufferManager(
                this.options.PacketSizeBuffer * this.options.TcpMaxConnections * 5,
                this.options.PacketSizeBuffer));

            if(this.tcpSocketListenerFactory == null)
                this.serviceCollection
                    .AddSingleton<ITcpSocketListenerFactory, DefaultTcpSocketListenerFactory>();

            if(this.udpSocketListenerFactory == null)
                this.serviceCollection
                    .AddSingleton<IUdpSocketListenerFactory, DefaultUdpSocketListenerFactory>();


            var serviceProvider = GetServiceProvider();

            return serviceProvider.GetService<IServer>();
        }

        public IServerBuilder SetMaximumConnections(int maxConnections)
        {
            this.options.TcpMaxConnections = maxConnections;
            return this;
        }

        public IServerBuilder UseTcpSocketListener<T>()
            where T: class, ITcpSocketListenerFactory
        {
            this.tcpSocketListenerFactory = typeof(T);
            this.serviceCollection.AddSingleton<ITcpSocketListenerFactory, T>();
            return this;
        }

        public IServerBuilder UseUdpSocketListener<T>()
            where T: class, IUdpSocketListenerFactory
        {
            this.udpSocketListenerFactory = typeof(T);
            this.serviceCollection.AddSingleton<IUdpSocketListenerFactory, T>();
            return this;
        }
    }
}