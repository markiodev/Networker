using System;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Example.ZeroFormatter.DefaultPackets
{
    public class ClientPingPacketHandler : PacketHandlerBase<PingPacket>
    {
        public override async Task Process(PingPacket packet, IPacketContext context)
        {
            var diff = DateTime.UtcNow.Subtract(packet.Time);
            Console.WriteLine($"Ping is {diff.Milliseconds}ms");
        }
    }
}