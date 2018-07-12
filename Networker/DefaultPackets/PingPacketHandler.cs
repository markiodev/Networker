using System;
using System.Threading.Tasks;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.DefaultPackets
{
    public class PingPacketHandler : PacketHandlerBase<PingPacket>
    {
        private readonly ILogger logger;

        public PingPacketHandler(ILogger logger, IPacketSerialiser serialiser)
            : base(serialiser)
        {
            this.logger = logger;
        }

        public override async Task Process(PingPacket packet, ISender sender)
        {
            sender.Send(new PingPacket
                        {
                            Time = packet.Time
                        });
        }
    }
}