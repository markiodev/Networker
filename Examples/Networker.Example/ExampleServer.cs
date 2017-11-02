using System;
using System.Collections.Generic;
using Networker.Interfaces;
using Networker.Server;

namespace Networker.Example
{
    public class ExampleServer : NetworkerServerBase
    {
        public ExampleServer(ServerConfiguration configuration,
            INetworkerLogger logger,
            IList<INetworkerPacketHandlerModule> modules, IContainerIoc container)
            : base(configuration, logger, modules, container)
        {
            this.ClientConnected += this.ClientConnectedEvent;
        }

        private void ClientConnectedEvent(object sender,
            TcpConnectionConnectedEventArgs NetworkerServerConnectionConnectedEventArgs)
        {
            this.Logger.Trace("A new connection was established, this is the event!");
        }
    }
}