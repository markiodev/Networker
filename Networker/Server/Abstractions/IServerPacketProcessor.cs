using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networker.Common.Abstractions;

namespace Networker.Server.Abstractions
{
	public interface IServerPacketProcessor
	{
		Task ProcessFromBuffer(ISender sender,
			byte[] buffer,
			int offset = 0,
			int length = 0,
			bool isTcp = true);

		void ProcessTcp(SocketAsyncEventArgs socketEvent);
		void ProcessUdp(SocketAsyncEventArgs socketEvent);
		void ProcessUdpFromBuffer(EndPoint sender, byte[] buffer, int offset = 0, int length = 0);
		void SetUdpSender(IUdpSocketSender sender);
	}
}