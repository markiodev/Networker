using System;

namespace Networker.Server
{
    public class ServerConfigurationAdvanced
    {
        public ServerConfigurationAdvanced()
        {
            this.MaxTcpConnections = 1000;
        }
        
        public int MaxTcpConnections { get; set; }
    }
}