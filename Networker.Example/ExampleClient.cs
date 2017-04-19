using System;
using Networker.Client;
using Networker.Interfaces;

namespace Networker.Example
{
    public class ExampleClient : NetworkerClientBase
    {
        public ExampleClient(ClientConfiguration clientConfiguration, INetworkerLogger logger)
            : base(clientConfiguration, logger) { }
    }
}