using System;
using Demo.ChatCommon;
using Networker;
using Networker.Extensions.Json;

namespace Demo.ChatClient
{
    public class MessageOfTheDayHandler : JsonPacketHandler<MessageOfTheDay>
    {
        public override void HandlePacket(IPacketContext packetContext, MessageOfTheDay packet)
        {
            Console.WriteLine($"MOTD was set by {packet.SetBy}: {packet.Message}");
        }
    }
}