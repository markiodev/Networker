using Networker;

namespace Demo.Common
{
    public class ChatMessage : PacketBase
    {
        public override int PacketTypeId => (int)PacketIdentifiers.ChatMessageToProcess;
        public string Message { get; set; }
    }
}