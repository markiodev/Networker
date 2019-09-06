using System;
using Demo.ChatCommon;
using Networker;
using Networker.Extensions.Json;

namespace Demo.ChatClient
{
    public class ChatMessageHandler : JsonPacketHandler<ChatMessage>
    {
        public override void HandlePacket(IPacketContext packetContext, ChatMessage packet)
        {
            Console.WriteLine($"[{DateTime.Now}] {packet.Name}: {packet.Message}");
        }
    }
}