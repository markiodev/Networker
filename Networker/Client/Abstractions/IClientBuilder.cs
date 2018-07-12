using System;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Client.Abstractions
{
    public interface IClientBuilder
    {
        IClient Build();

        IClientBuilder RegisterPacketHandler<TPacket, TPacketHandler>()
            where TPacket: PacketBase where TPacketHandler: IPacketHandler;

        IClientBuilder RegisterPacketHandlerModule<T>()
            where T: IPacketHandlerModule;

        IClientBuilder SetLogLevel(LogLevel logLevel);

        IClientBuilder UseIp(string ip);

        IClientBuilder UseLogger<T>()
            where T: class, ILogger;

        IClientBuilder UseTcp(int port);
        IClientBuilder UseUdp(int port, int localPort);
    }
}