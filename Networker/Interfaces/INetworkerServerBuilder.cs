using System;

namespace Networker.Interfaces
{
    public interface INetworkerServerBuilder
    {
        INetworkerServer Build<T>()
            where T: INetworkerServer;

        INetworkerServerBuilder RegisterLogger(INetworkerLoggerAdapter logAdapter);
        INetworkerServerBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>();
        INetworkerServerBuilder RegisterPacketHandlerModule<TPacketHandlerModule>();
        INetworkerServerBuilder UseIpAddresses(string[] ipAddresses);
        INetworkerServerBuilder UseTcp(int port);
        INetworkerServerBuilder UseUdp(int localPort, int? remotePort);
    }
}