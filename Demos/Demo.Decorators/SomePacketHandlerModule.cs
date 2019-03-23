using System;
using Networker.Common;

namespace Demo.Decorators
{
    public class SomePacketHandlerModule : PacketHandlerModuleBase
    {
        public SomePacketHandlerModule()
        {
            this.AddPacketHandler<SomePacket, SomePacketHandler>();
        }
    }
}