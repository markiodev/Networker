using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Networker.Common.Abstractions
{
    public abstract class BuilderBase<TBuilder, TResult, TBuilderOptions> : IBuilder<TBuilder, TResult> 
        where TBuilder : class, IBuilder<TBuilder, TResult>
        where TBuilderOptions : class, IBuilderOptions
    {
        //Modules
        protected PacketHandlerModule module;
        protected List<IPacketHandlerModule> modules;

        //Service Collection
        protected IServiceCollection serviceCollection;
        protected Func<IServiceProvider> serviceProviderFactory;

        //Builder Options
        protected TBuilderOptions options;

        private Type logger = typeof(NoOpLogger);

        public BuilderBase()
        {
            this.options = Activator.CreateInstance<TBuilderOptions>();
            this.serviceCollection = new ServiceCollection();
            this.modules = new List<IPacketHandlerModule>();
            this.module = new PacketHandlerModule();
            this.modules.Add(this.module);
        }

        public abstract TResult Build();

        public IServiceCollection GetServiceCollection()
        {
            return this.serviceCollection;
        }

        public TBuilder SetServiceCollection(IServiceCollection serviceCollection, Func<IServiceProvider> serviceProviderFactory = null)
        {
            this.serviceCollection = serviceCollection;
            this.serviceProviderFactory = serviceProviderFactory;
            return this as TBuilder;
        }

        public TBuilder RegisterPacketHandler<TPacket, TPacketHandler>()
            where TPacket : class
            where TPacketHandler : IPacketHandler
        {
            this.module.AddPacketHandler<TPacket, TPacketHandler>();
            return this as TBuilder;
        }

        public TBuilder RegisterPacketHandlerModule(IPacketHandlerModule packetHandlerModule)
        {
            this.modules.Add(packetHandlerModule);
            return this as TBuilder;
        }

        public TBuilder RegisterPacketHandlerModule<T>() where T : IPacketHandlerModule
        {
            this.modules.Add(Activator.CreateInstance<T>());
            return this as TBuilder;
        }

        public TBuilder SetLogLevel(LogLevel logLevel)
        {
            this.options.LogLevel = logLevel;
            return this as TBuilder;
        }

        public TBuilder SetPacketBufferSize(int size)
        {
            this.options.PacketSizeBuffer = size;
            return this as TBuilder;
        }

        public TBuilder UseLogger<T>() where T : class, ILogger
        {
            this.logger = typeof(T);
            return this as TBuilder;
        }

        public TBuilder UseLogger(ILogger logger)
        {
            this.logger = logger.GetType();
            return this as TBuilder;
        }

        public TBuilder UseTcp(int port)
        {
            this.options.TcpPort = port;
            return this as TBuilder;
        }

        public TBuilder UseUdp(int port)
        {
            this.options.UdpPort = port;
            return this as TBuilder;
        }

        protected void SetupSharedDependencies()
        {
            var packetHandlers = new PacketHandlers();
            foreach (var packetHandlerModule in this.modules)
            {
                foreach (var packetHandler in packetHandlerModule.GetPacketHandlers())
                {
                    this.serviceCollection.AddSingleton(packetHandler.Value);
                }
            }

            this.serviceCollection.AddSingleton<TBuilderOptions>(this.options);
            this.serviceCollection.AddSingleton(typeof(ILogger), logger);
            this.serviceCollection.AddSingleton<IPacketHandlers, PacketHandlers>();
            this.serviceCollection.AddSingleton<ILogLevelProvider, LogLevelProvider>();
        }

        protected IServiceProvider GetServiceProvider()
        {
            var serviceProvider = this.serviceProviderFactory != null ? serviceProviderFactory.Invoke() : this.serviceCollection.BuildServiceProvider();

            PacketSerialiserProvider.PacketSerialiser = serviceProvider.GetService<IPacketSerialiser>();
            serviceProvider.GetService<ILogLevelProvider>().SetLogLevel(this.options.LogLevel);

            IPacketHandlers packetHandlers = serviceProvider.GetService<IPacketHandlers>();
            foreach (var packetHandlerModule in this.modules)
            {
                foreach (var packetHandler in packetHandlerModule.GetPacketHandlers())
                {
                    packetHandlers.Add(PacketSerialiserProvider.PacketSerialiser.CanReadName ? packetHandler.Key.Name : "Default",
                        (IPacketHandler)serviceProvider.GetService(packetHandler.Value));
                }
            }

            if (!PacketSerialiserProvider.PacketSerialiser.CanReadName && packetHandlers.GetPacketHandlers().Count > 1)
            {
                throw new Exception("A PacketSerialiser which cannot identify a packet can only support up to one packet type");
            }

            return serviceProvider;
        }
    }
}
