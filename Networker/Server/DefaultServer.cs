using System;
using System.Collections.Generic;
using Networker.Interfaces;

namespace Networker.Server
{
    public class DefaultServer : NetworkerServerBase
    {
        public DefaultServer(ServerConfiguration configuration,
            INetworkerLogger logger,
            IList<INetworkerPacketHandlerModule> modules,
            IContainerIoc container)
            : base(configuration, logger, modules, container) { }
    }
}