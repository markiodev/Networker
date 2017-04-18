using System;
using SimpleNet.Common;
using SimpleNet.Interfaces;

namespace SimpleNet.Client
{
    public class SimpleNetClientBuilder : ISimpleNetClientBuilder
    {
        private readonly ClientConfiguration configuration;
        private readonly ISimpleNetLogger logger;

        public SimpleNetClientBuilder()
        {
            this.configuration = new ClientConfiguration();
            this.logger = new SimpleNetLogger();
        }

        public ISimpleNetClient Build<T>()
            where T: ISimpleNetClient
        {
            return (T)Activator.CreateInstance(typeof(T), this.configuration, this.logger);
        }

        public ISimpleNetClientBuilder RegisterLogger(ISimpleNetLoggerAdapter loggerAdapter)
        {
            this.logger.RegisterLogger(loggerAdapter);
            return this;
        }

        public ISimpleNetClientBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>()
        {
            this.logger.Trace(
                $"Registered packet handler {typeof(TPacketHandlerType).Name} for type {typeof(TPacketType).Name}.");
            this.configuration.PacketHandlers.Add(typeof(TPacketType).Name, typeof(TPacketHandlerType));
            return this;
        }

        public ISimpleNetClientBuilder RegisterPacketHandler(string packetName, Type handlerType)
        {
            this.logger.Trace($"Registered packet handler {packetName} for type {handlerType.Name}.");
            this.configuration.PacketHandlers.Add(packetName, handlerType);
            return this;
        }

        public ISimpleNetClientBuilder RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            this.logger.Trace($"Registered packet handler module {typeof(TPacketHandlerModule).Name}.");
            this.configuration.PacketHandlerModules.Add(typeof(TPacketHandlerModule));
            return this;
        }

        public ISimpleNetClientBuilder UseDryIoc()
        {
            return this;
        }

        public ISimpleNetClientBuilder UseIp(string ip)
        {
            this.configuration.Ip = ip;
            return this;
        }

        public ISimpleNetClientBuilder UseTcp(int port)
        {
            this.configuration.UseTcp = true;
            this.configuration.TcpPort = port;
            return this;
        }

        public ISimpleNetClientBuilder UseUdp(int remotePort, int? localPort)
        {
            this.configuration.UseUdp = true;
            this.configuration.UdpPortRemote = remotePort;
            this.configuration.UdpPortLocal = localPort ?? remotePort;
            return this;
        }
    }
}