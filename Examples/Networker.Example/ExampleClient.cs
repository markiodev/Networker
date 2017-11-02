using System;
using System.Collections.Generic;
using Networker.Client;
using Networker.Interfaces;

namespace Networker.Example
{
    public class ExampleClient : NetworkerClientBase
    {
        public ExampleClient(ClientConfiguration clientConfiguration,
            INetworkerLogger logger,
            IList<INetworkerPacketHandlerModule> packetHandlerModules)
            : base(clientConfiguration, logger, packetHandlerModules) { }
    }
}