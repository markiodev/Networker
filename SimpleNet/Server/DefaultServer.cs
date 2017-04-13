using System;
using SimpleNet.Interfaces;

namespace SimpleNet.Server
{
    public class DefaultServer : SimpleNetServerBase
    {
        public DefaultServer(ServerConfiguration configuration, ISimpleNetLogger logger)
            : base(configuration, logger) { }
    }
}