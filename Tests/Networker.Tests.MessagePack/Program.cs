using Microsoft.Extensions.Logging;
using Networker.Client;
using Networker.Extensions.MessagePack;
using Networker.Server;
using System;
using System.Threading;

namespace Networker.Tests.MessagePack 
{
	class Program 
	{
		static void Main(string[] args)
        {
            var server = new ServerBuilder().
				UseTcp(1000).
				RegisterPacketHandler<PingPacket, PingPacketHandler>().
                UseMessagePack().
                ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConsole();
                    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                }).
                Build();

            server.Start();

            var client = new ClientBuilder().
				UseIp("127.0.0.1").
				UseTcp(1000).
				UseMessagePack().
				Build();

            client.Connect();

			for (int i = 0; i < 10; i++) 
			{
				client.Send(new PingPacket(DateTime.Now));
				Thread.Sleep(1000);
			}
           
			Console.WriteLine("Done. Press any key to continue...");

            Console.ReadLine();
        }
	}
}
