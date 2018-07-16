using System;
using Microsoft.Extensions.DependencyInjection;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Server.Abstractions
{
    public interface IServerBuilder
    {
        IServer Build();
        IServiceCollection GetServiceCollection();

        IServerBuilder RegisterPacketHandler<TPacket, TPacketHandler>()
            where TPacket: class where TPacketHandler: IPacketHandler;

        IServerBuilder RegisterPacketHandlerModule(IPacketHandlerModule packetHandlerModule);

        IServerBuilder RegisterPacketHandlerModule<T>()
            where T: IPacketHandlerModule;

        IServerBuilder SetLogLevel(LogLevel logLevel);

        IServerBuilder SetServiceCollection(IServiceCollection serviceCollection);

        IServerBuilder UseLogger<T>()
            where T: class, ILogger;

        IServerBuilder UseLogger(ILogger logger);
        
        IServerBuilder UseTcp(int port);

        IServerBuilder UseTcpSocketListener<T>()
            where T: class, ITcpSocketListenerFactory;

        IServerBuilder UseUdp(int port);

        IServerBuilder UseUdpSocketListener<T>()
            where T: class, IUdpSocketListenerFactory;

        IServerBuilder SetMaximumConnections(int maxConnections);
        IServerBuilder SetPacketBufferSize(int packetBufferSize);
    }
}