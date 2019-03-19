using System.Net.Sockets;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class TcpConnection : ITcpConnection
	{
		public TcpConnection(Socket socket)
		{
			Socket = socket;
		}

		public Socket Socket { get; set; }
	}
}