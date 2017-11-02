using System;
using Networker.Client;
using Networker.Example.Packets;
using Networker.Interfaces;

namespace Networker.Example.ClientPacketHandlers
{
    public class ChatMessageReceivedPacketHandler : PacketHandlerBase<ChatMessageReceivedPacket>
    {
        private readonly INetworkerLogger _logger;

        public ChatMessageReceivedPacketHandler(INetworkerLogger logger)
        {
            this._logger = logger;
        }

        public override void Handle(ChatMessageReceivedPacket packet)
        {
            this._logger.Trace($"I got a response from server - {packet.Message}");
        }
    }
}