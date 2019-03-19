using Networker.Common;

namespace Networker.Example.ProtoBuf
{
	public class DefaultPacketHandlerModule : PacketHandlerModuleBase
	{
		public DefaultPacketHandlerModule()
		{
			AddPacketHandler<PingPacket, PingPacketHandler>();
		}
	}
}