using System;
using System.Threading;
using Networker.Example.Packets;
using Networker.Example.ServerPacketHandlers;
using Networker.Helpers;
using Networker.Server;

namespace Networker.Example
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var server = new NetworkerServerBuilder().UseConsoleLogger()
                                                     .SetPacketBufferSize(1500)
                                                     .SetUdpSocketPoolSize(5000)
                                                     .UseIpAddresses(new[] {"127.0.0.1"})
                                                     .UseTcp(1000)
                                                     .UseUdp(1001, 1002)
                                                     .RegisterPacketHandler<ChatMessageDispatchPacket,
                                                         ChatMessageDispatchPacketHandler>()
                                                     .RegisterPacketHandler<ServerInformationRequestPacket,
                                                         ServerInformationRequestPacketHandler>()
                                                     .Build<ExampleServer>()
                                                     .Start();

            server.ClientConnected += (sender, eventArgs) => { Console.WriteLine("Connected"); };

            server.ClientDisconnected += (sender, eventArgs) => { Console.WriteLine("Disconnected"); };

            int timesProcessed = 0;
            while(timesProcessed < 10)
            {
                server.Broadcast(new ServerInformationResponsePacket
                                 {
                                     MachineName = Environment.MachineName
                                 });
                timesProcessed++;
                Thread.Sleep(1000);
            }

            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }
    }
}