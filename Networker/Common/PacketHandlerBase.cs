using Networker.Common.Abstractions;
using System.Threading.Tasks;

namespace Networker.Common
{
    public abstract class PacketHandlerBase<T> : IPacketHandler
        where T : class
    {
        public async Task Handle(IPacketContext context)
        {
            await this.Process(context.Serialiser.Deserialise<T>(context.PacketBytes), context);
        }

        public abstract Task Process(T packet, IPacketContext context);
    }
}