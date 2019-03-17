using Networker.Common;

namespace Networker.Example.MessagePack
{
    public class ExamplePacketHandlerModule : PacketHandlerModuleBase
    {
        public ExamplePacketHandlerModule()
        {
            this.AddPacketHandler<TestPacketThing, TestPacketHandler>();
            this.AddPacketHandler<TestPacketOtherThing, TestPacketHandler>();
        }
    }
}