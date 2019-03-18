using System;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Example.ZeroFormatter
{
    public class TestPacketTwoHandler : PacketHandlerBase<TestPacketOtherThing>
    {
        public override async Task Process(TestPacketOtherThing packet, IPacketContext context) { }
    }
}