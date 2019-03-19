using System.Net;
using Networker.Client.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Client
{
	public class UdpClientSender : IUdpSocketSender
	{
		private readonly IClient _client;

		public UdpClientSender(IClient client, IClientPacketProcessor clientPacketProcessor)
		{
			_client = client;
			clientPacketProcessor.SetUdpSocketSender(this);
		}

		public void Broadcast<T>(T packet)
		{
			_client.SendUdp(packet);
		}

		public void SendTo(byte[] packetBytes, IPEndPoint endpoint)
		{
			_client.SendUdp(packetBytes);
		}

		public void SendTo<T>(T packet, IPEndPoint endpoint)
		{
			_client.SendUdp(packet);
		}
	}
}