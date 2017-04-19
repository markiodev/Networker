using System;
using Networker.Interfaces;

namespace Networker.Server
{
    public class DefaultServer : NetworkerServerBase
    {
        public DefaultServer(ServerConfiguration configuration, INetworkerLogger logger)
            : base(configuration, logger) { }
    }
}