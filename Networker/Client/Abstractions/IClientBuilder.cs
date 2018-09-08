using System;
using Microsoft.Extensions.DependencyInjection;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Client.Abstractions
{
    public interface IClientBuilder
    {
        IClient Build();

        IClientBuilder RegisterPacketHandler<TPacket, TPacketHandler>()
            where TPacket: class where TPacketHandler: IPacketHandler;

        IClientBuilder RegisterPacketHandlerModule<T>()
            where T: IPacketHandlerModule;

        IClientBuilder SetLogLevel(LogLevel logLevel);

        IClientBuilder UseIp(string ip);

        IClientBuilder UseLogger<T>()
            where T: class, ILogger;

        IClientBuilder UseLogger(ILogger logger);
        IClientBuilder UseTcp(int port);
        IClientBuilder UseUdp(int port, int localPort);
        IClientBuilder UseUdp(int port);
        IClientBuilder SetPacketBufferSize(int size);
        IClientBuilder SetPacketBufferPoolSize(int size);
        IServiceCollection GetServiceCollection();
        IClientBuilder SetServiceCollection(IServiceCollection serviceCollection, Func<IServiceProvider> serviceProviderFactory = null);
    }
}