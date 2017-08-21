using System;
using System.Threading;
using System.Threading.Tasks;
using Networker.Client;
using Networker.Common;
using Networker.Example.ClientPacketHandlers;
using Networker.Example.Packets;
using Networker.Example.ServerPacketHandlers;
using Networker.Helpers;
using Networker.Interfaces;
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
            /*
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

            for(var i = 0; i < 100; i++)
            {
                Task.Factory.StartNew(() =>
                                      {
                                          client.Send(new ChatMessageDispatchPacket
                                                      {
                                                          Message =
                                                              "Performance Testing",
                                                          Sender = "System"
                                                      });
                                      });
            }

            for(var i = 0; i < 1000; i++)
            {
                client.SendAndHandleResponse(new ServerInformationRequestPacket(),
                    new Action<ServerInformationResponsePacket>(e =>
                                                                {
                                                                    client
                                                                        .Container.Resolve<INetworkerLogger>()
                                                                        .Trace(
                                                                            $"I am sync, my transaction ID is {e.TransactionId}. {e.MachineName}");
                                                                }));
            }
            /*
            client.SendAndHandleResponse(new ServerInformationRequestPacket());

            client.SendAndHandleResponseAsync(new ServerInformationRequestPacket(),
                new Action<ServerInformationResponsePacket>(e =>
                                                            {
                                                                client.Container.Resolve<INetworkerLogger>()
                                                                      .Trace(
                                                                          $"I am async, my transaction ID is {e.TransactionId}. {e.MachineName}");
                                                            }));*/
        }
    }
}