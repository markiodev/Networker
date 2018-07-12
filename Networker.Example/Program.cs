using System;
using System.Threading;
using System.Threading.Tasks;
using Networker.Client;
using Networker.Common;
using Networker.DefaultPackets;
using Networker.Server;

namespace Networker.V3.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerBuilder().UseTcp(1000)
                                            .UseUdp(5000)
                                            .UseLogger<ConsoleLogger>()
                                            //.SetLogLevel(LogLevel.Info)
                                            .RegisterPacketHandlerModule<DefaultPacketHandlerModule>()
                                            .RegisterPacketHandlerModule<ExamplePacketHandlerModule>()
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
                                                    .UseUdp(5000, 5000 + i + 1)
                                                    .RegisterPacketHandler<PingPacket,
                                                        ClientPingPacketHandler>()
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