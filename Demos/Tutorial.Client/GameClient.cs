using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Networker.Client;
using Networker.Client.Abstractions;
using Networker.Extensions.ProtobufNet;
using Tutorial.Common;

namespace Tutorial.Client
{
	public class GameClient
	{
		public GameClient()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("clientSettings.json", false, true)
				.Build();
 
			var networkerSettings = config.GetSection("Networker");
 
			Client = new ClientBuilder()
				.UseIp(networkerSettings.GetValue<string>("Address"))
				.UseTcp(networkerSettings.GetValue<int>("TcpPort"))
				.UseUdp(networkerSettings.GetValue<int>("UdpPort"))
				.ConfigureLogging(loggingBuilder =>
				{
					loggingBuilder.AddConfiguration(config.GetSection("Logging"));
					loggingBuilder.AddConsole();
				})
				.UseProtobufNet()
				.RegisterPacketHandler<ChatPacket, ClientChatPacketHandler>()
				.Build();
		}
 
		public IClient Client { get; set; }
	}
}