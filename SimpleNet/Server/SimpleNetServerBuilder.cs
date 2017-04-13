using System;
using SimpleNet.Common;
using SimpleNet.Interfaces;

namespace SimpleNet.Server
{
    public class SimpleNetServerBuilder : ISimpleNetServerBuilder
    {
        private readonly ServerConfiguration configuration;
        private readonly ISimpleNetLogger logger;

        public SimpleNetServerBuilder()
        {
            this.configuration = new ServerConfiguration();
            this.logger = new SimpleNetLogger();
        }

        public ISimpleNetServer Build<T>()
            where T: ISimpleNetServer
        {
            return (T)Activator.CreateInstance(typeof(T), this.configuration, this.logger);
        }

        public ISimpleNetServerBuilder RegisterLogger(ISimpleNetLoggerAdapter logAdapter)
        {
            this.logger.RegisterLogger(logAdapter);
            return this;
        }

        public ISimpleNetServerBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>()
        {
            this.logger.Trace(
                $"Registered packet handler {typeof(TPacketHandlerType).Name} for type {typeof(TPacketType).Name}.");
            this.configuration.PacketHandlers.Add(typeof(TPacketType).Name, typeof(TPacketHandlerType));
            return this;
        }

        public ISimpleNetServerBuilder RegisterPacketHandler(string packetName, Type handlerType)
        {
            this.logger.Trace($"Registered packet handler {packetName} for type {handlerType.Name}.");
            this.configuration.PacketHandlers.Add(packetName, handlerType);
            return this;
        }

        public ISimpleNetServerBuilder RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            this.logger.Trace($"Registered packet handler module {typeof(TPacketHandlerModule).Name}.");
            this.configuration.PacketHandlerModules.Add(typeof(ISimpleNetPacketBaseHandlerModule));
            return this;
        }

        public ISimpleNetServerBuilder UseIpAddresses(string[] ipAddresses)
        {
            this.configuration.IpAddresses = ipAddresses;
            return this;
        }

        public ISimpleNetServerBuilder UseTcp(int port)
        {
            this.configuration.UseTcp = true;
            this.configuration.TcpPort = port;
            return this;
        }

        public ISimpleNetServerBuilder UseUdp(int localPort, int? remotePort)
        {
            this.configuration.UseUdp = true;
            this.configuration.UdpPortRemote = remotePort ?? localPort;
            this.configuration.UdpPortLocal = localPort;
            return this;
        }
    }
}