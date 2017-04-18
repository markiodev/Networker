using System;
using SimpleNet.Client;
using SimpleNet.Interfaces;

namespace SimpleNet.Example
{
    public class ChatMessageReceivedPacketHandler : PacketHandlerBase<ChatMessageReceivedPacket
    >
    {
        private readonly ISimpleNetLogger _logger;

        public ChatMessageReceivedPacketHandler(ISimpleNetLogger logger)
        {
            this._logger = logger;
        }

        public override void Handle(ChatMessageReceivedPacket packet)
        {
            this._logger.Trace($"I got a response from server - {packet.Message}");
        }
    }
}