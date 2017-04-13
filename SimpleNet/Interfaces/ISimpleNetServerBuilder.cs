using System;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetServerBuilder
    {
        ISimpleNetServer Build<T>()
            where T: ISimpleNetServer;

        ISimpleNetServerBuilder RegisterLogger(ISimpleNetLoggerAdapter logAdapter);
        ISimpleNetServerBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>();
        ISimpleNetServerBuilder RegisterPacketHandler(string packetName, Type handlerType);
        ISimpleNetServerBuilder RegisterPacketHandlerModule<TPacketHandlerModule>();
        ISimpleNetServerBuilder UseIpAddresses(string[] ipAddresses);
        ISimpleNetServerBuilder UseTcp(int port);
        ISimpleNetServerBuilder UseUdp(int localPort, int? remotePort);
    }
}