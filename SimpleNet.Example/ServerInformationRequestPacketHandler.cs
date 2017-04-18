using System;
using SimpleNet.Interfaces;
using SimpleNet.Server;

namespace SimpleNet.Example
{
    public class
        ServerInformationRequestPacketHandler : ServerPacketHandlerBase<
            ServerInformationRequestPacket>
    {
        public override void Handle(ISimpleNetConnection sender, ServerInformationRequestPacket packet)
        {
            sender.CreatePacket(new ServerInformationResponsePacket
                                {
                                    MachineName = Environment.MachineName
                                })
                  .Send();
        }
    }
}