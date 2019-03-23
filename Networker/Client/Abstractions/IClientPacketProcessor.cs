using System.Net.Sockets;
using Networker.Server.Abstractions;

namespace Networker.Client.Abstractions
{
	public interface IClientPacketProcessor
	{
		void Process(Socket socket);
		void Process(UdpReceiveResult data);
		void SetUdpSocketSender(IUdpSocketSender socketSender);
	}
}