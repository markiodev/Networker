using System;

namespace Networker.Interfaces
{
    public interface INetworkerClientBuilder
    {
        INetworkerClient Build<T>()
            where T: INetworkerClient;

        INetworkerClientBuilder RegisterLogger(INetworkerLoggerAdapter loggerAdapter);
        INetworkerClientBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>();
        INetworkerClientBuilder RegisterPacketHandlerModule<TPacketHandlerModule>();
        INetworkerClientBuilder UseDryIoc();
        INetworkerClientBuilder UseIp(string ip);
        INetworkerClientBuilder UseTcp(int port);
        INetworkerClientBuilder UseUdp(int remotePort, int? localPort);
    }
}