using System;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Example.ZeroFormatter
{
    public class TestPacketOneHandler : PacketHandlerBase<TestPacketThing>
    {
	    public override async Task Process(TestPacketThing packet, IPacketContext context)
        {
        }
    }
}