using System;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetClientBuilder
    {
        ISimpleNetClient Build<T>()
            where T: ISimpleNetClient;

        ISimpleNetClientBuilder RegisterLogger(ISimpleNetLoggerAdapter loggerAdapter);
        ISimpleNetClientBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>();
        ISimpleNetClientBuilder RegisterPacketHandler(string packetName, Type handlerType);
        ISimpleNetClientBuilder RegisterPacketHandlerModule<TPacketHandlerModule>();
        ISimpleNetClientBuilder UseDryIoc();
        ISimpleNetClientBuilder UseIp(string ip);
        ISimpleNetClientBuilder UseTcp(int port);
        ISimpleNetClientBuilder UseUdp(int remotePort, int? localPort);
    }
}