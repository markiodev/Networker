using System;
using Networker.Interfaces;
using Networker.Server;

namespace Networker.Example
{
    public class ChatMessageDispatchPacketHandler : ServerPacketHandlerBase<ChatMessageDispatchPacket
    >
    {
        private readonly INetworkerLogger _logger;

        public ChatMessageDispatchPacketHandler(INetworkerLogger logger)
        {
            this._logger = logger;
        }

        public override void Handle(INetworkerConnection sender, ChatMessageDispatchPacket packet)
        {
            this._logger.Trace($"Chat Message Handled: {packet.Sender} - {packet.Message}");

            sender?.Send(new ChatMessageReceivedPacket
                         {
                             Message = "Got it, thanks",
                             UniqueKey = "ChatMessageReceivedPacket"
                         });
        }
    }
}