using System;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class TcpConnectionConnectedEventArgs : EventArgs
	{
		public TcpConnectionConnectedEventArgs(ITcpConnection connection)
		{
			Connection = connection;
		}

		public ITcpConnection Connection { get; }
	}
}