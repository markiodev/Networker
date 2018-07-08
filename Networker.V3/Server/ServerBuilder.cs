using Microsoft.Extensions.DependencyInjection;
using Networker.V3.Common;

namespace Networker.V3.Server
{
    public class ServerBuilder : IServerBuilder
    {
        public IServerBuilder UseTcp(int port)
        {
            return null;
        }

        public IServerBuilder UseUdp(int port)
        {
            return null;
        }

        public IServerBuilder UseLogger(ILogger logger)
        {
            return null;
        }

        public IServerBuilder SetServiceCollection(IServiceCollection serviceCollection)
        {
            return null;
        }

        public IServerBuilder RegisterPacketHandlerModule(IServerPacketHandlerModule packetHandlerModule)
        {
            return null;
        }

        public IServerBuilder RegisterPacketHandlerModule<T>()
            where T: IServerPacketHandlerModule
        {
            return null;
        }

        public IServer Build()
        {
            return null;
        }
    }
}