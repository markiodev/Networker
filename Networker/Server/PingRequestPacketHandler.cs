using System;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Server
{
    public class PingRequestPacketHandler : ServerPacketHandlerBase<PingRequestPacket>
    {
        public override void Handle(INetworkerConnection sender, PingRequestPacket packet)
        {
            sender.Send(new PingResponsePacket
                        {
                            TransactionId = packet.TransactionId
                        });
        }
    }
}