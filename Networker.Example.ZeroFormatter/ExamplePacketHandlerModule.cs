using System;
using Networker.Common;

namespace Networker.Example.ZeroFormatter
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