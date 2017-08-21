using System.Collections.Generic;
using Networker.Interfaces;

namespace Networker.Server
{
    public class TcpConnectionsProvider : ITcpConnectionsProvider
    {
        private readonly List<TcpConnection> connections;

        public TcpConnectionsProvider(List<TcpConnection> connections)
        {
            this.connections = connections;
        }

        public List<TcpConnection> Provide()
        {
            return this.connections;
        }
    }
}