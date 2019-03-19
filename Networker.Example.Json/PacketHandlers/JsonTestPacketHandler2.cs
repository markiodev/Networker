using Networker.Common.Abstractions;
using Networker.Example.Json.Packets;

namespace Networker.Example.Json.PacketHandlers
{
	public class JsonTestPacketHandler2 : DynamicPacketHandlerBase,
		IDynamicPacketHandler<JsonTestPacket>,
		IDynamicPacketHandler<JsonTestPacketChild>
	{
		public void Process(JsonTestPacket packet, IPacketContext packetContext)
		{
		}

		public void Process(JsonTestPacketChild packet, IPacketContext packetContext)
		{
		}
	}
}