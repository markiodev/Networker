using Microsoft.Extensions.Logging;

namespace Networker.Common.Abstractions
{
	public interface IBuilderOptions
	{
		LogLevel LogLevel { get; set; }
		int PacketSizeBuffer { get; set; }
		int TcpPort { get; set; }
		int UdpPort { get; set; }
	}
}