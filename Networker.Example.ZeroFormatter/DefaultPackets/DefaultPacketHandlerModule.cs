using Networker.Common;

namespace Networker.Example.ZeroFormatter.DefaultPackets
{
	public class DefaultPacketHandlerModule : PacketHandlerModuleBase
	{
		public DefaultPacketHandlerModule()
		{
			AddPacketHandler<PingPacket, PingPacketHandler>();
		}
	}
}