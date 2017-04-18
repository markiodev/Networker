using System;
using SimpleNet.Interfaces;
using SimpleNet.Server;

namespace SimpleNet.Example
{
    public class ExampleServer : SimpleNetServerBase
    {
        public ExampleServer(ServerConfiguration configuration, ISimpleNetLogger logger)
            : base(configuration, logger)
        {
            this.ClientConnected += this.ClientConnectedEvent;
        }

        private void ClientConnectedEvent(object sender,
            TcpConnectionConnectedEventArgs simpleNetServerConnectionConnectedEventArgs)
        {
            this.Logger.Trace("A new connection was established, this is the event!");
        }
    }
}