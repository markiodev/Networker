using System;
using SimpleNet.Interfaces;
using SimpleNet.Server;

namespace SimpleNet.Example
{
    public class ExampleServer : SimpleNetServerBase
    {
        public ExampleServer(ServerConfiguration configuration, ISimpleNetLogger logger)
            : base(configuration, logger) { }
    }
}