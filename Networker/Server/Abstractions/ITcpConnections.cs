using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Networker.Server.Abstractions
{
    public interface ITcpConnections
    {
        ITcpConnection Add(Socket connection);
        List<ITcpConnection> GetConnections();
        void Remove(ITcpConnection connection);
    }
}