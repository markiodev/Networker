using System;
using SimpleNet.Interfaces;
using SimpleNet.Server;

namespace SimpleNet.Example
{
    public class ChatMessageDispatchPacketHandler : SimpleNetServerPacketHandlerBase<ChatMessageDispatchPacket
    >
    {
        private readonly ISimpleNetLogger _logger;

        public ChatMessageDispatchPacketHandler(ISimpleNetLogger logger)
        {
            this._logger = logger;
        }

        public override void Handle(ISimpleNetConnection sender, ChatMessageDispatchPacket packet)
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