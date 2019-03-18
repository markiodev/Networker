using Networker.Common.Abstractions;

namespace Networker.Example.Json.PacketHandlers
{
	public interface IDynamicPacketHandler<T> where T : class
	{
		void Process(T packet, IPacketContext packetContext);
	}
}