using System;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class TcpConnectionDisconnectedEventArgs : EventArgs
	{
		public TcpConnectionDisconnectedEventArgs(ITcpConnection connection)
		{
			Connection = connection;
		}

		public ITcpConnection Connection { get; }
	}
}