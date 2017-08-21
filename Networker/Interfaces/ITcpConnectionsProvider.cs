using System.Collections.Generic;
using Networker.Server;

namespace Networker.Interfaces
{
    public interface ITcpConnectionsProvider
    {
        List<TcpConnection> Provide();
    }
}