using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class ServerInformation : IServerInformation
	{
		public ServerInformation()
		{
			IsRunning = true;
		}

		public int InvalidTcpPackets { get; set; }
		public int InvalidUdpPackets { get; set; }
		public bool IsRunning { get; set; }
		public int ProcessedTcpPackets { get; set; }
		public int ProcessedUdpPackets { get; set; }
	}
}