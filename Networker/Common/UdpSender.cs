using System.Net;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Common
{
	public class UdpSender : ISender
	{
		private readonly IUdpSocketSender _socketSender;

		public UdpSender(IUdpSocketSender socketSender)
		{
			_socketSender = socketSender;
		}

		public EndPoint RemoteEndpoint { get; set; }

		public IPEndPoint EndPoint => RemoteEndpoint as IPEndPoint;

		public void Send<T>(T packet)
		{
			_socketSender.SendTo(packet, EndPoint);
		}
	}
}