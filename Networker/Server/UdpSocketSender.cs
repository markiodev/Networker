using System.Net;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class UdpSocketSender : IUdpSocketSender
	{
		private readonly IPacketSerialiser _packetSerialiser;
		private readonly IUdpSocketListener _udpSocketListener;

		public UdpSocketSender(IUdpSocketListener udpSocketListener, IPacketSerialiser packetSerialiser)
		{
			_udpSocketListener = udpSocketListener;
			_packetSerialiser = packetSerialiser;
		}

		public void Broadcast<T>(T packet)
		{
			var socket = _udpSocketListener.GetSocket();
			var serialisedPacket = _packetSerialiser.Serialise(packet);
			socket.Send(serialisedPacket);
		}

		public void SendTo(byte[] packetBytes, IPEndPoint endpoint)
		{
			var socket = _udpSocketListener.GetSocket();
			socket.SendTo(packetBytes, endpoint);
		}

		public void SendTo<T>(T packet, IPEndPoint endpoint)
		{
			var socket = _udpSocketListener.GetSocket();
			var serialisedPacket = _packetSerialiser.Serialise(packet);

			socket.SendTo(serialisedPacket, endpoint);
		}
	}
}