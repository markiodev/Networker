using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Networker.Common.Abstractions
{
    public interface IBuilder<TBuilder, TResult>
    {
        //Build
        TResult Build();

        //Service Collection
        IServiceCollection GetServiceCollection();
        TBuilder SetServiceCollection(IServiceCollection serviceCollection, Func<IServiceProvider> serviceProviderFactory = null);

        //Packet Handler
        TBuilder RegisterPacketHandler<TPacket, TPacketHandler>()
            where TPacket : class where TPacketHandler : IPacketHandler;
        TBuilder RegisterPacketHandlerModule(IPacketHandlerModule packetHandlerModule);
        TBuilder RegisterPacketHandlerModule<T>()
            where T : IPacketHandlerModule;
        
        //Microsoft Stuff
        TBuilder RegisterTypes(Action<IServiceCollection> serviceCollection);
        TBuilder ConfigureLogging(Action<ILoggingBuilder> loggingBuilder);
        TBuilder UseConfiguration(IConfiguration configuration);
        TBuilder UseConfiguration<T>(IConfiguration configuration) where T : class;

        //Tcp
        TBuilder UseTcp(int port);

        //Udp
        TBuilder UseUdp(int port);

        //Info
        TBuilder SetPacketBufferSize(int size);
    }
}
