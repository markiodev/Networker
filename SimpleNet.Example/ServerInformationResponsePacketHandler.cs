using System;
using SimpleNet.Client;
using SimpleNet.Interfaces;

namespace SimpleNet.Example
{
    public class ServerInformationResponsePacketHandler : PacketHandlerBase<
        ServerInformationResponsePacket>
    {
        private readonly ISimpleNetLogger _logger;

        public ServerInformationResponsePacketHandler(ISimpleNetLogger logger)
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