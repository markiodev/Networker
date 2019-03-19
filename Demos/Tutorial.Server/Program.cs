using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Networker.Extensions.ProtobufNet;
using Networker.Server;
using Tutorial.Client;
using Tutorial.Common;

namespace Tutorial.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", false, true)
				.Build();
 
			var networkerSettings = config.GetSection("Networker");
 
			var server = new ServerBuilder()
				.UseTcp(networkerSettings.GetValue<int>("TcpPort"))
				.UseUdp(networkerSettings.GetValue<int>("UdpPort"))
				.ConfigureLogging(loggingBuilder =>
				{
					loggingBuilder.AddConfiguration(config.GetSection("Logging"));
					loggingBuilder.AddConsole();
				})
				.UseProtobufNet()
				.RegisterPacketHandler<ChatPacket, ChatPacketHandler>()
				.Build();
 
			server.Start();
 
			var gameClient = new GameClient();
			gameClient.Client.Connect();
 
			while (server.Information.IsRunning)
			{
				gameClient.Client.Send(new ChatPacket
				{
					Message = DateTime.Now.ToString()
				});

				Thread.Sleep(10000);
			}
		}
	}
}
