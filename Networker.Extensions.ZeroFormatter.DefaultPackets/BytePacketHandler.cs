using System.Threading.Tasks;
using Networker.Common.Abstractions;

namespace Networker.Extensions.ZeroFormatter.DefaultPackets
{
	public class BytePacketHandler : IPacketHandler<BytePacket>
	{
		public Task Handle(IPacketContext packetContext)
		{
			return null;
		}
	}
}