using System;
using System.Threading;
using System.Threading.Tasks;
using Networker.Client;
using Networker.DefaultPackets;

namespace Networker.V3.Example.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            for(var i = 0; i < 1000; i++)
            {
                try
                {
                    var client = new ClientBuilder().UseIp("127.0.0.1")
                                                    .UseTcp(1000)
                                                    .UseUdp(5000, 5000 + i + 1)
                                                    .Build();

                    client.Connect();

                    Task.Factory.StartNew(() =>
                                          {
                                              while(true)
                                              {
                                                  client.Send(new PingPacket());
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

            Console.ReadLine();
        }
    }
}