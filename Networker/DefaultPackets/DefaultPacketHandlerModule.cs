using System;
using Networker.Common;

namespace Networker.DefaultPackets
{
    public class DefaultPacketHandlerModule : PacketHandlerModuleBase
    {
        public DefaultPacketHandlerModule()
        {
            this.AddPacketHandler<PingPacket, PingPacketHandler>();
        }
    }
}