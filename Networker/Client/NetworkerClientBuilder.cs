using System;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Client
{
    public class NetworkerClientBuilder : INetworkerClientBuilder
    {
        private readonly ClientConfiguration configuration;
        private readonly INetworkerLogger logger;

        public NetworkerClientBuilder()
        {
            this.configuration = new ClientConfiguration();
            this.logger = new NetworkerLogger();
        }

        public INetworkerClient Build<T>()
            where T: INetworkerClient
        {
            return (T)Activator.CreateInstance(typeof(T), this.configuration, this.logger);
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
            this.configuration.PacketHandlers.Add(typeof(TPacketType).Name, typeof(TPacketHandlerType));
            return this;
        }

        public INetworkerClientBuilder RegisterPacketHandler(string packetName, Type handlerType)
        {
            this.logger.Trace($"Registered packet handler {packetName} for type {handlerType.Name}.");
            this.configuration.PacketHandlers.Add(packetName, handlerType);
            return this;
        }

        public INetworkerClientBuilder RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            this.logger.Trace($"Registered packet handler module {typeof(TPacketHandlerModule).Name}.");
            this.configuration.PacketHandlerModules.Add(typeof(TPacketHandlerModule));
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