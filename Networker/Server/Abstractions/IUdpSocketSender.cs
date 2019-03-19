using System.Net;

namespace Networker.Server.Abstractions
{
	public interface IUdpSocketSender
	{
		void Broadcast<T>(T packet);
		void SendTo(byte[] packetBytes, IPEndPoint endpoint);
		void SendTo<T>(T packet, IPEndPoint endpoint);
	}
}