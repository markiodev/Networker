using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class ServerBuilder : IServerBuilder
    {
        private readonly PacketHandlerModule module;
        private readonly List<IPacketHandlerModule> modules;
        private readonly ServerBuilderOptions options;
        private Type logger;
        private IServiceCollection serviceCollection;
        private Type tcpSocketListenerFactory;
        private Type udpSocketListenerFactory;

        public ServerBuilder()
        {
            this.options = new ServerBuilderOptions();
            this.serviceCollection = new ServiceCollection();
            this.modules = new List<IPacketHandlerModule>();
            this.module = new PacketHandlerModule();
            this.modules.Add(this.module);
        }

        public IServer Build()
        {
            var packetHandlers = new PacketHandlers();
            foreach(var packetHandlerModule in this.modules)
            {
                foreach(var packetHandler in packetHandlerModule.GetPacketHandlers())
                {
                    this.serviceCollection.AddSingleton(packetHandler.Value);
                }
            }

            this.serviceCollection.AddSingleton(this.options);
            this.serviceCollection.AddSingleton<IPacketHandlers>(packetHandlers);
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

            if(this.logger == null)
                this.serviceCollection.AddSingleton<ILogger>(new NoOpLogger());

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            foreach(var packetHandlerModule in this.modules)
            {
                foreach(var packetHandler in packetHandlerModule.GetPacketHandlers())
                {
                    packetHandlers.Add(packetHandler.Key.Name,
                        (IPacketHandler)serviceProvider.GetService(packetHandler.Value));
                }
            }

            return serviceProvider.GetService<IServer>();
        }

        public IServiceCollection GetServiceCollection()
        {
            return this.serviceCollection;
        }

        public IServerBuilder RegisterPacketHandler<TPacket, TPacketHandler>()
            where TPacket: class where TPacketHandler: IPacketHandler
        {
            this.module.AddPacketHandler<TPacket, TPacketHandler>();
            return this;
        }

        public IServerBuilder RegisterPacketHandlerModule(IPacketHandlerModule packetHandlerModule)
        {
            this.modules.Add(packetHandlerModule);
            return this;
        }

        public IServerBuilder RegisterPacketHandlerModule<T>()
            where T: IPacketHandlerModule
        {
            this.modules.Add(Activator.CreateInstance<T>());
            return this;
        }

        public IServerBuilder SetLogLevel(LogLevel logLevel)
        {
            this.options.LogLevel = logLevel;
            return this;
        }

        public IServerBuilder SetMaximumConnections(int maxConnections)
        {
            this.options.TcpMaxConnections = maxConnections;
            return this;
        }

        public IServerBuilder SetPacketBufferSize(int packetBufferSize)
        {
            this.options.PacketSizeBuffer = packetBufferSize;
            return this;
        }

        public IServerBuilder SetServiceCollection(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
            return this;
        }

        public IServerBuilder UseLogger<T>()
            where T: class, ILogger
        {
            this.logger = typeof(T);
            this.serviceCollection.AddSingleton<ILogger, T>();
            return this;
        }

        public IServerBuilder UseLogger(ILogger logger)
        {
            this.logger = logger.GetType();
            this.serviceCollection.AddSingleton<ILogger>(logger);
            return this;
        }

        public IServerBuilder UseTcp(int port)
        {
            this.options.TcpPort = port;
            return this;
        }

        public IServerBuilder UseTcpSocketListener<T>()
            where T: class, ITcpSocketListenerFactory
        {
            this.tcpSocketListenerFactory = typeof(T);
            this.serviceCollection.AddSingleton<ITcpSocketListenerFactory, T>();
            return this;
        }

        public IServerBuilder UseUdp(int port)
        {
            this.options.UdpPort = port;
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