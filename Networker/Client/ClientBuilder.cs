using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Networker.Client.Abstractions;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server;
using Networker.Server.Abstractions;

namespace Networker.Client
{
    public class ClientBuilder : IClientBuilder
    {
        private readonly List<IPacketHandlerModule> modules;
        private readonly ClientBuilderOptions options;
        private readonly ServiceCollection serviceCollection;
        private Type logger;
        private readonly PacketHandlerModule module;

        public ClientBuilder()
        {
            this.options = new ClientBuilderOptions();
            this.serviceCollection = new ServiceCollection();
            this.modules = new List<IPacketHandlerModule>();
            this.module = new PacketHandlerModule();
            this.modules.Add(this.module);
        }

        public IClient Build()
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
            this.serviceCollection.AddSingleton<IClient, Client>();
            this.serviceCollection.AddSingleton<IPacketSerialiser, ZeroFormatterPacketSerialiser>();
            this.serviceCollection.AddSingleton<IClientPacketProcessor, ClientPacketProcessor>();
            this.serviceCollection.AddSingleton<IPacketHandlers>(packetHandlers);

            if(this.logger == null)
                this.serviceCollection.AddSingleton<ILogger, NoOpLogger>();

            var serviceProvider = this.serviceCollection.BuildServiceProvider();

            foreach (var packetHandlerModule in this.modules)
            {
                foreach (var packetHandler in packetHandlerModule.GetPacketHandlers())
                {
                    packetHandlers.Add(packetHandler.Key.Name,
                        (IPacketHandler)serviceProvider.GetService(packetHandler.Value));
                }
            }

            return serviceProvider.GetService<IClient>();
        }

        public IClientBuilder RegisterPacketHandler<TPacket, TPacketHandler>()
            where TPacket: PacketBase where TPacketHandler: IPacketHandler
        {
            this.module.AddPacketHandler<TPacket, TPacketHandler>();
            return this;
        }

        public IClientBuilder RegisterPacketHandlerModule<T>()
            where T: IPacketHandlerModule
        {
            this.modules.Add(Activator.CreateInstance<T>());
            return this;
        }

        public IClientBuilder SetLogLevel(LogLevel logLevel)
        {
            this.options.LogLevel = logLevel;
            return this;
        }

        public IClientBuilder UseIp(string ip)
        {
            this.options.Ip = ip;
            return this;
        }

        public IClientBuilder UseLogger<T>()
            where T: class, ILogger
        {
            this.logger = typeof(T);
            this.serviceCollection.AddSingleton<ILogger, T>();
            return this;
        }

        public IClientBuilder UseTcp(int port)
        {
            this.options.TcpPort = port;
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