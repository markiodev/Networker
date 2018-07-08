using System;
using System.Threading;
using Networker.Client;
using Networker.Common;
using Networker.Example.ClientPacketHandlers;
using Networker.Example.Packets;
using Networker.Helpers;

namespace Networker.Example.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new NetworkerClientBuilder().UseConsoleLogger()
                                                       .UseIp("127.0.0.1")
                                                       .UseTcp(1000)
                                                       .UseUdp(1001, 1002)
                                                       .RegisterPacketHandler<ChatMessageReceivedPacket,
                                                           ChatMessageReceivedPacketHandler>()
                                                       .RegisterPacketHandler<ServerInformationResponsePacket,
                                                           ServerInformationResponsePacketHandler>()
                                                       .Build<ExampleClient>()
                                                       .Connect();

            client.Connected += (sender, socket) => { Console.WriteLine("Client connected event invoked"); };

            client.Disconnected += (sender, socket) =>
            {
                Console.WriteLine("Client disconnected event invoked");
            };

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

            int count = 0;

            new Thread(new ThreadStart(() =>
            {
                for (var j = 0; j < 3; j++)
                {
                    new Thread(new ThreadStart(() =>
                    {
                        while (true)
                        {
                            Interlocked.Increment(ref count);
                            client.Send(
                                new
                                ChatMessageDispatchPacket
                                {
                                    Message =
                                        "Time to spam some UDP"
                                        + count,
                                    Sender =
                                        "UDP Spam" + count
                                },
                                NetworkerProtocol.Udp);
                            Thread.Sleep(1);
                        }
                    })).Start();
                }
            })).Start();

            Console.ReadLine();
            Console.ReadLine();
        }
    }
}
