using System;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Example.ZeroFormatter
{
    public class TestPacketOneHandler : PacketHandlerBase<TestPacketThing>
    {
        public TestPacketOneHandler(IPacketSerialiser packetSerialiser)
            : base(packetSerialiser) { }

        
        public override async Task Process(TestPacketThing packet, IPacketContext context)
        {
        }
    }
}