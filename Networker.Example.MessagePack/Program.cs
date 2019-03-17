using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Client;
using Networker.Example.MessagePack.DefaultPackets;
using Networker.Formatter.MessagePack;
using Networker.Server;

namespace Networker.Example.MessagePack
{
	class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerBuilder()
                            .UseTcp(1000)
                            .UseUdp(5000)
							.RegisterPacketHandlerModule<DefaultPacketHandlerModule>()
                            .RegisterPacketHandlerModule<ExamplePacketHandlerModule>()
                            .UseMessagePack()
                            .ConfigureLogging(loggingBuilder =>
                                                {
                                                    loggingBuilder.AddConsole();
                                                    loggingBuilder.SetMinimumLevel(
                                                        LogLevel.Debug);
                                                })
                            .Build();

            server.Start();

            server.ServerInformationUpdated += (sender, eventArgs) =>
                                               {
                                                   var dateTime = DateTime.UtcNow;

                                                   Console.WriteLine(
                                                       $"{dateTime} {eventArgs.ProcessedTcpPackets} TCP Packets Processed");
                                                   Console.WriteLine(
                                                       $"{dateTime} {eventArgs.InvalidTcpPackets} Invalid or Lost TCP Packets");
                                                   Console.WriteLine(
                                                       $"{dateTime} {eventArgs.ProcessedUdpPackets} UDP Packets Processed");
                                                   Console.WriteLine(
                                                       $"{dateTime} {eventArgs.InvalidUdpPackets} Invalid or Lost UDP Packets");
                                                   Console.WriteLine(
                                                       $"{dateTime} {eventArgs.TcpConnections} TCP connections active");
                                               };
            server.ClientConnected += (sender, eventArgs) =>
                                      {
                                          Console.WriteLine(
                                              $"Client Connected - {eventArgs.Connection.Socket.RemoteEndPoint}");
                                      };
            server.ClientDisconnected += (sender, eventArgs) =>
                                         {
                                             Console.WriteLine(
                                                 $"Client Disconnected - {eventArgs.Connection.Socket.RemoteEndPoint}");
                                         };

            for(var i = 0; i < 10; i++)
            {
                try
                {
                    var client = new ClientBuilder().UseIp("127.0.0.1")
                                                    .UseTcp(1000)
                                                    .UseUdp(5000)
                                                    .RegisterPacketHandler<PingPacket,
                                                        ClientPingPacketHandler>()
                                                    .UseMessagePack()
                                                    .Build();

                    client.Connect();

                    Task.Factory.StartNew(() =>
                                          {
                                              while(true)
                                              {
                                                  client.Send(new PingPacket
                                                              {
                                                                  Time = DateTime.UtcNow
                                                              });

                                                  client.Send(new TestPacketThing());
                                                  client.SendUdp(new TestPacketOtherThing());
                                                  Thread.Sleep(10);
                                              }
                                          });
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Task.Factory.StartNew(() =>
                                  {
                                      while(true)
                                      {
                                          server.Broadcast(new PingPacket
                                                           {
                                                               Time = DateTime.UtcNow
                                                           });

                                          Thread.Sleep(10000);
                                      }
                                  });

            Console.ReadLine();
        }
    }
}