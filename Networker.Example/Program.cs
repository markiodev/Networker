using System;
using System.Reflection;
using System.Threading;
using Networker.Client;
using Networker.Common;
using Networker.Helpers;
using Networker.Server;

namespace Networker.Example
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
                                                     .RegisterPacketHandler<ServerInformationRequestPacket,
                                                         ServerInformationRequestPacketHandler>()
                                                     //.AutoRegisterPacketHandlers(typeof(ChatMessageDispatchPacketHandler).GetTypeInfo().Assembly)
                                                     .Build<ExampleServer>()
                                                     .Start();

            var client = new NetworkerClientBuilder().UseConsoleLogger()
                                                     .UseIp("127.0.0.1")
                                                     .UseTcp(1000)
                                                     .UseUdp(1001, 1002)
                                                     .RegisterPacketHandler<ChatMessageReceivedPacket,
                                                         ChatMessageReceivedPacketHandler>()
                                                     .RegisterPacketHandler<ServerInformationResponsePacket,
                                                         ServerInformationResponsePacketHandler>()
                                                     //.AutoRegisterPacketHandlers(typeof(ChatMessageDispatchPacketHandler).GetTypeInfo().Assembly)
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

            new Thread(() =>
                       {
                           while(true)
                           {
                               server.Broadcast(new ServerInformationResponsePacket
                                                {
                                                    MachineName =
                                                        Environment
                                                            .MachineName
                                                });

                               Thread.Sleep(5000);
                           }
                       }).Start();

            /*client.CreatePacket(new ServerInformationRequestPacket())
                .HandleResponse<ServerInformationResponsePacket>(
                    packet => Console.WriteLine($"I am sync. {packet.MachineName}")).Send();

            client.CreatePacket(new ServerInformationRequestPacket())
                .HandleResponseAsync<ServerInformationResponsePacket>(
                    packet => Console.WriteLine($"I am async. {packet.MachineName}")).Send();*/
        }
    }
}