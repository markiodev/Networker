using System;
using SimpleNet.Interfaces;

namespace SimpleNet.Client
{
    public class DefaultClient : SimpleNetClientBase
    {
        public DefaultClient(ClientConfiguration clientConfiguration, ISimpleNetLogger logger)
            : base(clientConfiguration, logger) { }
    }
}