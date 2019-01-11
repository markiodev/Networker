using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Client;
using Networker.Common;
using Networker.Formatter.ProtobufNet;
using Networker.Server;

namespace Networker.Example.ProtoBuf
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerBuilder().UseTcp(1000)
                                            .SetMaximumConnections(6000)
                                            .UseUdp(5000)
                                            //.UseLogger<ConsoleLogger>()
                                            .SetLogLevel(LogLevel.Information)
                                            .RegisterPacketHandlerModule<DefaultPacketHandlerModule>()
                                            .UseProtobufNet()
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