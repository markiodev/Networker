using System.Net;
using System.Net.Sockets;
using Networker.Common.Abstractions;

namespace Networker.Common
{
	public class TcpSender : ISender
	{
		private readonly IPacketSerialiser _packetSerialiser;

		public TcpSender(IPacketSerialiser packetSerialiser)
		{
			_packetSerialiser = packetSerialiser;
		}

		public Socket Socket { get; set; }

		public IPEndPoint EndPoint => Socket.RemoteEndPoint as IPEndPoint;

		public void Send<T>(T packet)
		{
			Socket.Send(_packetSerialiser.Serialise(packet));
		}
	}
}