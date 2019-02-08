using System;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Example.ZeroFormatter
{
    public class TestPacketTwoHandler : PacketHandlerBase<TestPacketOtherThing>
    {
        public TestPacketTwoHandler(IPacketSerialiser packetSerialiser)
            : base(packetSerialiser) { }

        public override async Task Process(TestPacketOtherThing packet, IPacketContext context) { }
    }
}