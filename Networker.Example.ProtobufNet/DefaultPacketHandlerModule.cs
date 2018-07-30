using System;
using Networker.Common;

namespace Networker.Example.ProtoBuf
{
    public class DefaultPacketHandlerModule : PacketHandlerModuleBase
    {
        public DefaultPacketHandlerModule()
        {
            this.AddPacketHandler<PingPacket, PingPacketHandler>();
        }
    }
}