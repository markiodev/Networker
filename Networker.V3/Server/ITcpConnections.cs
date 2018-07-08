using System;
using System.Collections.Generic;

namespace Networker.V3.Server
{
    public interface ITcpConnections
    {
        List<ITcpConnections> GetConnections();
    }
}