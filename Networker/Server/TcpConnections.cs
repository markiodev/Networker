using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Networker.Server
{
    public class TcpConnections : ITcpConnections
    {
        private readonly IPacketSerialiser packetSerialiser;
        private readonly List<ITcpConnection> connections;
        private readonly ObjectPool<ITcpConnection> objectPool;

        public TcpConnections(
            ServerBuilderOptions options,
            IPacketSerialiser packetSerialiser)
        {
            this.packetSerialiser = packetSerialiser;
            this.objectPool = new ObjectPool<ITcpConnection>(options.TcpMaxConnections);

            for (var i = 0; i < this.objectPool.Capacity; i++)
            {
                this.objectPool.Push(new TcpConnection(null));
            }

            this.connections = new List<ITcpConnection>();
        }

        public List<ITcpConnection> GetConnections()
        {
            lock (this.connections)
            {
                return this.connections;
            }
        }

        public void Broadcast<T>(T packet)
        {
            var packetBytes = packetSerialiser.Serialise(packet);
            foreach (var connection in GetConnections())
            {
                connection.Socket.Send(packetBytes);
            }
        }

        public ITcpConnection FindByEndpoint(EndPoint endpoint)
        {
            foreach (var connection in GetConnections())
            {
                if (connection.Socket.RemoteEndPoint == endpoint)
                {
                    return connection;
                }
            }
            return null;
        }

        public ITcpConnection Add(Socket socket)
        {
            var connection = this.objectPool.Pop();
            connection.Socket = socket;

            lock (this.connections)
            {
                this.connections.Add(connection);
            }

            return connection;
        }

        public void Remove(ITcpConnection connection)
        {
            lock (this.connections)
            {
                this.connections.Remove(connection);
            }

            connection.Socket = null;
            this.objectPool.Push(connection);
        }
    }
}
