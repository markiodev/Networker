using System;
using Networker.Client;
using Networker.Example.Packets;
using Networker.Interfaces;

namespace Networker.Example.ClientPacketHandlers
{
    public class ServerInformationResponsePacketHandler : PacketHandlerBase<ServerInformationResponsePacket>
    {
        private readonly INetworkerLogger _logger;

        public ServerInformationResponsePacketHandler(INetworkerLogger logger)
        {
            this._logger = logger;
        }

        public override void Handle(ServerInformationResponsePacket packet)
        {
            this._logger.Trace(
                $"Received UDP Packet containing server information. Machine Name: {packet.MachineName}");
        }
    }
}