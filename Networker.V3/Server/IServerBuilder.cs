using Microsoft.Extensions.DependencyInjection;
using Networker.V3.Common;

namespace Networker.V3.Server
{
    public interface IServerBuilder
    {
        IServerBuilder UseTcp(int port);
        IServerBuilder UseUdp(int port);
        IServerBuilder UseLogger(ILogger logger);
        IServerBuilder SetServiceCollection(IServiceCollection serviceCollection);
        IServerBuilder RegisterPacketHandlerModule(IServerPacketHandlerModule packetHandlerModule);
        IServerBuilder RegisterPacketHandlerModule<T>() where T : IServerPacketHandlerModule;
        IServer Build();
    }
}