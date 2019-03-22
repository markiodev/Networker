using System.Collections.Generic;
using System.Net.Sockets;

namespace Networker.Server.Abstractions
{
    public interface ITcpConnections
    {
        List<ITcpConnection> GetConnections();

        ITcpConnection Add(Socket connection);
        void Remove(ITcpConnection connection);
    }
}
