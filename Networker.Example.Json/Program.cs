using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Client;
using Networker.Example.Json.Middleware;
using Networker.Example.Json.PacketHandlers;
using Networker.Example.Json.Packets;
using Networker.Extensions.Json;
using Networker.Server;

namespace Networker.Example.Json
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var server = new ServerBuilder().UseTcp(1000)
				.UseUdp(5000)
				.UseJson()
				.ConfigureLogging(loggingBuilder =>
				{
					loggingBuilder.AddConsole();
					loggingBuilder.SetMinimumLevel(
						LogLevel.Debug);
				})
				//.RegisterMiddleware<RoleCheckMiddleware>()
				.RegisterPacketHandler<JsonTestPacket,
					JsonTestPacketHandler<JsonTestPacket>>()
				.RegisterPacketHandler<JsonTestPacketChild,
					JsonTestPacketHandler<JsonTestPacketChild>>()
				.RegisterPacketHandler<JsonTestBanPlayerPacket,
					JsonTestBanPlayerPacketHandler
				>() //Runs same logic as above, but only if you are an admin!
				//OR we could do this
				//.RegisterPacketHandler<JsonTestPacket, JsonTestPacketHandler2>()
				//.RegisterPacketHandler<JsonTestPacketChild, JsonTestPacketHandler2>()
				.Build();

			server.Start();

			try
			{
				var client = new ClientBuilder().UseIp("127.0.0.1")
					.UseTcp(1000)
					.UseUdp(5000)
					.UseJson()
					.Build();

				client.Connect();

				Task.Factory.StartNew(() =>
				{
					while (true)
					{
						client.Send(new JsonTestPacket());
						client.Send(new JsonTestPacketChild());
						Thread.Sleep(1000);
					}
				});
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			Console.ReadLine();
			Console.ReadLine();
		}
	}
}