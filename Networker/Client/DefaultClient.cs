using System;
using System.Collections.Generic;
using Networker.Interfaces;

namespace Networker.Client
{
    public class DefaultClient : NetworkerClientBase
    {
        public DefaultClient(ClientConfiguration clientConfiguration,
            INetworkerLogger logger,
            IList<INetworkerPacketHandlerModule> packetHandlerModules,
            IContainerIoc container)
            : base(clientConfiguration, logger, packetHandlerModules, container) { }
    }
}