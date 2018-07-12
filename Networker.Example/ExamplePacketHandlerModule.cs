using System;
using Networker.Common;

namespace Networker.V3.Example
{
    public class ExamplePacketHandlerModule : PacketHandlerModuleBase
    {
        public ExamplePacketHandlerModule()
        {
            this.AddPacketHandler<TestPacketThing, TestPacketOneHandler>();
            this.AddPacketHandler<TestPacketOtherThing, TestPacketTwoHandler>();
        }
    }
}