using System;

namespace Networker.Server
{
    public class ServerConfigurationAdvanced
    {
        public ServerConfigurationAdvanced()
        {
            this.ConnectionPollIntervalMs = 10;
            this.IncomingSocketPollIntervalMs = 10;
            this.MaxTcpConnections = 1000;
        }

        public int ConnectionPollIntervalMs { get; set; }
        public int IncomingSocketPollIntervalMs { get; set; }
        public int MaxTcpConnections { get; set; }
    }
}