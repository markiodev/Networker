using System;
using Networker;

namespace Demo.ChatCommon
{
    public class ChatMessage : PacketBase
    {
        public override int PacketTypeId => (int)PacketIdentifiers.ChatMessage;
        public string Message { get; set; }
        public string Name { get; set; }
    }
}
