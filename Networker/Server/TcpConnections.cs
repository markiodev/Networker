using System.Collections.Generic;
using System.Net.Sockets;
using Networker.Common;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class TcpConnections : ITcpConnections
	{
		private readonly List<ITcpConnection> connections;
		private readonly ObjectPool<ITcpConnection> objectPool;

		public TcpConnections(ServerBuilderOptions options)
		{
			objectPool = new ObjectPool<ITcpConnection>(options.TcpMaxConnections);

			for (var i = 0; i < objectPool.Capacity; i++) objectPool.Push(new TcpConnection(null));

			connections = new List<ITcpConnection>();
		}

		public ITcpConnection Add(Socket socket)
		{
			var connection = objectPool.Pop();
			connection.Socket = socket;

			lock (connections)
			{
				connections.Add(connection);
			}

			return connection;
		}

		public List<ITcpConnection> GetConnections()
		{
			lock (connections)
			{
				return connections;
			}
		}

		public void Remove(ITcpConnection connection)
		{
			lock (connections)
			{
				connections.Remove(connection);
			}

			connection.Socket = null;
			objectPool.Push(connection);
		}
	}
}