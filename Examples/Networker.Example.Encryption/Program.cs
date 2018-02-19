using System;
using Networker.Client;
using Networker.Common;
using Networker.Example.Encryption.Packets;
using Networker.Helpers;
using Networker.Server;

namespace Networker.Example.Encryption
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var server = new NetworkerServerBuilder().UseConsoleLogger()
                                                     .UseIpAddresses(new[] {"127.0.0.1"})
                                                     .UseTcp(1000)
                                                     .UseUdp(1001, 1002)
                                                     .RegisterPacketHandler<ChatMessageDispatchPacket,
                                                         ChatMessageDispatchPacketHandler>()
                                                     .Build<ExampleServer>()
                                                     .Start();

            var client = new NetworkerClientBuilder().UseConsoleLogger()
                                                     .UseIp("127.0.0.1")
                                                     .UseTcp(1000)
                                                     .UseUdp(1001, 1002)
                                                     .RegisterPacketHandler<ChatMessageReceivedPacket,
                                                         ChatMessageReceivedPacketHandler>()
                                                     .Build<ExampleClient>()
                                                     .Connect();




            client.Send(new ChatMessageDispatchPacket
                        {
                            Message = "I am the message",
                            Sender = "The Sender"
                        });

            client.Send(new ChatMessageDispatchPacket
                        {
                            Message = "I am a UDP message",
                            Sender = "The Sender (UDP)"
                        },
                NetworkerProtocol.Udp);
        }
    }
}