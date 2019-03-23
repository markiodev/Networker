using System.Threading.Tasks;

namespace Networker.Common.Abstractions
{
    public interface IPacketHandler
    {
        Task Handle(IPacketContext packetContext);
    }
}
