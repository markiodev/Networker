using Microsoft.Extensions.Logging;
using Networker.Common.Abstractions;

namespace Networker.Server
{
	public class ServerBuilderOptions : IBuilderOptions
	{
		public ServerBuilderOptions()
		{
			TcpMaxConnections = 100;
			UdpSocketObjectPoolSize = 10;
			PacketSizeBuffer = 5000;
			LogLevel = LogLevel.Error;
		}

		public int TcpMaxConnections { get; set; }
		public int UdpSocketObjectPoolSize { get; set; }

		public LogLevel LogLevel { get; set; }
		public int PacketSizeBuffer { get; set; }

		public int TcpPort { get; set; }
		public int UdpPort { get; set; }
	}
}