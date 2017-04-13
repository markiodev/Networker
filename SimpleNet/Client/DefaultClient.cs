using System;
using SimpleNet.Interfaces;

namespace SimpleNet.Client
{
    public class DefaultClient : SimpleNetClientBase
    {
        public DefaultClient(SimpleNetClientConfiguration clientConfiguration, ISimpleNetLogger logger)
            : base(clientConfiguration, logger) { }
    }
}