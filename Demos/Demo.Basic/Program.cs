using System;
using System.Threading;
using System.Threading.Tasks;
using Demo.Common;
using Microsoft.Extensions.Logging;
using Networker.Client;
using Networker.Extensions.Json;
using Networker.Server;

namespace Demo.Basic
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
				.RegisterPacketHandler<BasicPacket, BasicPacketHandler>()
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
						client.Send(new BasicPacket());
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