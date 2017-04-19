using System;
using Networker.Interfaces;

namespace Networker.Client
{
    public class DefaultClient : NetworkerClientBase
    {
        public DefaultClient(ClientConfiguration clientConfiguration, INetworkerLogger logger)
            : base(clientConfiguration, logger) { }
    }
}