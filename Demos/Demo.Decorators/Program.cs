using Microsoft.Extensions.Logging;
using Networker.Server;
using System;

namespace Demo.Decorators 
{
	class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerBuilder().
				UseTcp(1000).
				SetMaximumConnections(6000).
				UseUdp(5000).
				ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.SetMinimumLevel(
                        LogLevel.Debug);
                }).
				RegisterPacketHandlerModule<SomePacketHandlerModule>().
				Build();

            server.Start();
           
            Console.ReadLine();
        }
    }
}