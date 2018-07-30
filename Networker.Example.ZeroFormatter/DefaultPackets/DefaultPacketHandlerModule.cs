using System;
using Networker.Common;

namespace Networker.Example.ZeroFormatter.DefaultPackets
{
public class DefaultPacketHandlerModule : PacketHandlerModuleBase
{
    public DefaultPacketHandlerModule()
    {
        this.AddPacketHandler<PingPacket, PingPacketHandler>();
    }
}
}