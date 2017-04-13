using System;
using SimpleNet.Client;
using SimpleNet.Interfaces;

namespace SimpleNet.Example
{
    public class ExampleClient : SimpleNetClientBase
    {
        public ExampleClient(SimpleNetClientConfiguration clientConfiguration, ISimpleNetLogger logger)
            : base(clientConfiguration, logger) { }
    }
}