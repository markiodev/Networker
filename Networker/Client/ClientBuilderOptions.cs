using Microsoft.Extensions.Logging;
using Networker.Common.Abstractions;

namespace Networker.Client
{
	public class ClientBuilderOptions : IBuilderOptions
	{
		public ClientBuilderOptions()
		{
			LogLevel = LogLevel.Error;
			PacketSizeBuffer = 3500;
			ObjectPoolSize = 200;
		}

		public string Ip { get; set; }
		public int ObjectPoolSize { get; set; }
		public int UdpPortLocal { get; set; }
		public LogLevel LogLevel { get; set; }
		public int PacketSizeBuffer { get; set; }

		public int TcpPort { get; set; }
		public int UdpPort { get; set; }
	}
}