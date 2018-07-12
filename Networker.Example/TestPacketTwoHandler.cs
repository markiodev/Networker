using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server;

namespace Networker.V3.Example
{
    public class TestPacketTwoHandler : PacketHandlerBase<TestPacketOtherThing>
    {
        public TestPacketTwoHandler(IPacketSerialiser packetSerialiser)
            : base(packetSerialiser) { }
        
        public override async Task Process(TestPacketOtherThing packet, ISender sender)
        {
        }
    }
}