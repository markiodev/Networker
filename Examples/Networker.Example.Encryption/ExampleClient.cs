using System;
using System.Collections.Generic;
using Networker.Client;
using Networker.Interfaces;

namespace Networker.Example.Encryption
{
    public class ExampleClient : NetworkerClientBase
    {
        public ExampleClient(ClientConfiguration clientConfiguration,
            INetworkerLogger logger,
            IList<INetworkerPacketHandlerModule> packetHandlerModules,
            IContainerIoc container)
            : base(clientConfiguration, logger, packetHandlerModules, container) { }
    }
}