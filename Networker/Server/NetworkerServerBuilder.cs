using System;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Server
{
    public class NetworkerServerBuilder : INetworkerServerBuilder
    {
        private readonly ServerConfiguration configuration;
        private readonly INetworkerLogger logger;

        public NetworkerServerBuilder()
        {
            this.configuration = new ServerConfiguration();
            this.logger = new NetworkerLogger();
        }

        public INetworkerServer Build<T>()
            where T: INetworkerServer
        {
            return (T)Activator.CreateInstance(typeof(T), this.configuration, this.logger);
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
            this.configuration.PacketHandlers.Add(typeof(TPacketType).Name, typeof(TPacketHandlerType));
            return this;
        }

        public INetworkerServerBuilder RegisterPacketHandler(string packetName, Type handlerType)
        {
            this.logger.Trace($"Registered packet handler {packetName} for type {handlerType.Name}.");
            this.configuration.PacketHandlers.Add(packetName, handlerType);
            return this;
        }

        public INetworkerServerBuilder RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            this.logger.Trace($"Registered packet handler module {typeof(TPacketHandlerModule).Name}.");
            this.configuration.PacketHandlerModules.Add(typeof(INetworkerPacketBaseHandlerModule));
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