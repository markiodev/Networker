using System;
using Networker.Helpers;
using Networker.Server;
using Networker.Example.Chat.Packets;

namespace Networker.Example.Chat.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new NetworkerServerBuilder().UseConsoleLogger()
                                                     .UseIpAddresses(new[] { "127.0.0.1" })
                                                     .UseTcp(1000)
                                                     .RegisterPacketHandler<ChatMessagePacket, ChatMessagePacketHandler>()
                                                     .Build<DefaultServer>()
                                                     .Start();
        }
    }
}
