using System;
using System.Collections.Generic;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Server
{
    public class NetworkerServerBuilder : INetworkerServerBuilder
    {
        private readonly ServerConfiguration configuration;
        private readonly INetworkerLogger logger;
        private readonly IList<INetworkerPacketHandlerModule> modules;
        private readonly DefaultPacketHandlerModule packetHandlerModule;

        public NetworkerServerBuilder()
        {
            this.configuration = new ServerConfiguration();
            this.logger = new NetworkerLogger();
            this.packetHandlerModule = new DefaultPacketHandlerModule();
            this.modules = new List<INetworkerPacketHandlerModule>();
            this.modules.Add(this.packetHandlerModule);
        }

        public INetworkerServer Build<T>()
            where T: INetworkerServer
        {
            return (T)Activator.CreateInstance(typeof(T), this.configuration, this.logger, this.modules);
        }

        public INetworkerServerBuilder RegisterLogger(INetworkerLoggerAdapter logAdapter)
        {
            this.logger.RegisterLogger(logAdapter);
            return this;
        }

        public INetworkerServerBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>()
        {
            this.logger.Trace(
                $"Registered packet handler {typeof(TPacketHandlerType).Name} for type {typeof(TPacketType).Name}.");
            this.packetHandlerModule.Modules.Add(typeof(TPacketType), typeof(TPacketHandlerType));
            return this;
        }

        public INetworkerServerBuilder RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            this.logger.Trace($"Registered packet handler module {typeof(TPacketHandlerModule).Name}.");
            var module =
                (INetworkerPacketHandlerModule)Activator.CreateInstance(typeof(TPacketHandlerModule));
            this.modules.Add(module);

            return this;
        }

        public INetworkerServerBuilder UseIpAddresses(string[] ipAddresses)
        {
            this.configuration.IpAddresses = ipAddresses;
            return this;
        }

        public INetworkerServerBuilder UseTcp(int port)
        {
            this.configuration.UseTcp = true;
            this.configuration.TcpPort = port;
            return this;
        }

        public INetworkerServerBuilder UseUdp(int localPort, int? remotePort)
        {
            this.configuration.UseUdp = true;
            this.configuration.UdpPortRemote = remotePort ?? localPort;
            this.configuration.UdpPortLocal = localPort;
            return this;
        }
    }
}