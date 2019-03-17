using Networker.Common;

namespace Networker.Example.MessagePack.DefaultPackets
{
	public class DefaultPacketHandlerModule : PacketHandlerModuleBase
	{
		public DefaultPacketHandlerModule()
		{
			this.AddPacketHandler<PingPacket, PingPacketHandler>();
		}
	}
}