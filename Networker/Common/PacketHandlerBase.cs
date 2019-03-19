using System.Threading.Tasks;
using Networker.Common.Abstractions;

namespace Networker.Common
{
	public abstract class PacketHandlerBase<T> : IPacketHandler
		where T : class
	{
		public async Task Handle(IPacketContext packetContext)
		{
			await Process(packetContext.GetPacket<T>(), packetContext);
		}

		public abstract Task Process(T packet, IPacketContext packetContext);
	}
}