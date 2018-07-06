using System;

namespace Networker.Server
{
    public class ServerConfigurationAdvanced
    {
        public ServerConfigurationAdvanced()
        {
            this.MaxTcpConnections = 1000;
            this.PacketBufferSize = 500;
            this.RegisterPacketHandlersAsSingletons = true;
            this.UdpSocketPoolSize = 1000;
        }
        
        public int MaxTcpConnections { get; set; }
        public int PacketBufferSize { get; set; }
        public bool RegisterPacketHandlersAsSingletons { get; set; }
        public int UdpSocketPoolSize { get; set; }
    }
}