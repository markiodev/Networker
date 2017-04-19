using System;
using Networker.Interfaces;
using Networker.Server;

namespace Networker.Example
{
    public class
        ServerInformationRequestPacketHandler : ServerPacketHandlerBase<
            ServerInformationRequestPacket>
    {
        public override void Handle(INetworkerConnection sender, ServerInformationRequestPacket packet)
        {
            sender.Send(new ServerInformationResponsePacket
                                {
                                    MachineName = Environment.MachineName
                                });
        }
    }
}