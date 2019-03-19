namespace Networker.Server.Abstractions
{
	public interface IServerInformation
	{
		int InvalidTcpPackets { get; set; }
		int InvalidUdpPackets { get; set; }
		bool IsRunning { get; set; }
		int ProcessedTcpPackets { get; set; }
		int ProcessedUdpPackets { get; set; }
	}
}