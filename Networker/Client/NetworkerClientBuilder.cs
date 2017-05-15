using System;
using System.Collections.Generic;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Client
{
    public class NetworkerClientBuilder : INetworkerClientBuilder
    {
        private readonly ClientConfiguration configuration;
        private readonly INetworkerLogger logger;
        private readonly IList<INetworkerPacketHandlerModule> modules;
        private readonly DefaultPacketHandlerModule packerHandlerModule;

        public NetworkerClientBuilder()
        {
            this.configuration = new ClientConfiguration();
            this.logger = new NetworkerLogger();
            this.packerHandlerModule = new DefaultPacketHandlerModule();
            this.modules = new List<INetworkerPacketHandlerModule>();
            this.modules.Add(this.packerHandlerModule);
        }

        public INetworkerClient Build<T>()
            where T: INetworkerClient
        {
            return (T)Activator.CreateInstance(typeof(T), this.configuration, this.logger, this.modules);
        }

        public INetworkerClientBuilder RegisterLogger(INetworkerLoggerAdapter loggerAdapter)
        {
            this.logger.RegisterLogger(loggerAdapter);
            return this;
        }

        public INetworkerClientBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>()
        {
            this.logger.Trace(
                $"Registered packet handler {typeof(TPacketHandlerType).Name} for type {typeof(TPacketType).Name}.");

            this.packerHandlerModule.Modules.Add(typeof(TPacketType), typeof(TPacketHandlerType));
            return this;
        }

        public INetworkerClientBuilder RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            this.logger.Trace($"Registered packet handler module {typeof(TPacketHandlerModule).Name}.");

            var module =
                (INetworkerPacketHandlerModule)Activator.CreateInstance(typeof(TPacketHandlerModule));

            this.modules.Add(module);
            return this;
        }

        public INetworkerClientBuilder UseDryIoc()
        {
            return this;
        }

        public INetworkerClientBuilder UseIp(string ip)
        {
            this.configuration.Ip = ip;
            return this;
        }

        public INetworkerClientBuilder UseTcp(int port)
        {
            this.configuration.UseTcp = true;
            this.configuration.TcpPort = port;
            return this;
        }

        public INetworkerClientBuilder UseUdp(int remotePort, int? localPort)
        {
            this.configuration.UseUdp = true;
            this.configuration.UdpPortRemote = remotePort;
            this.configuration.UdpPortLocal = localPort ?? remotePort;
            return this;
        }
    }
}