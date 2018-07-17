using System;
using System.Threading;
using System.Threading.Tasks;
using Networker.Client;
using Networker.Example.ProtoBuf;
using Networker.Formatter.ProtobufNet;

namespace Networker.Example.Protobuf.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            for(var i = 0; i < 5000; i++)
            {
                try
                {
                    var client = new ClientBuilder().UseIp("127.0.0.1")
                                                    .UseTcp(1000)
                                                    .UseUdp(5000)
                                                    .RegisterPacketHandler<PingPacket,
                                                        ClientPingPacketHandler>()
                                                    .UseProtobufNet()
                                                    .Build();

                    client.Connected += (sender, socket) =>
                                        {
                                            Console.WriteLine(
                                                $"Client has connected to {socket.RemoteEndPoint}");
                                        };

                    client.Disconnected += (sender, socket) =>
                                        {
                                            Console.WriteLine(
                                                $"Client has disconnected from {socket.RemoteEndPoint}");
                                        };

                    client.Connect();

                    Task.Factory.StartNew(() =>
                                          {
                                              while(true)
                                              {
                                                  client.Send(new PingPacket
                                                              {
                                                                  Time = DateTime.UtcNow
                                                              });

                                                  client.SendUdp(new PingPacket
                                                                 {
                                                                     Time = DateTime.UtcNow
                                                                 });

                                                  Thread.Sleep(10);
                                              }
                                          });
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.ReadLine();
        }
    }
}