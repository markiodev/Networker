using System.Net.Sockets;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server;

namespace Networker.V3.Example
{
    public class TestPacketOneHandler : PacketHandlerBase<TestPacketThing>
    {
        public TestPacketOneHandler(IPacketSerialiser packetSerialiser)
            : base(packetSerialiser) { }


        public override async Task Process(TestPacketThing packet, ISender socket)
        {
        }
    }
}