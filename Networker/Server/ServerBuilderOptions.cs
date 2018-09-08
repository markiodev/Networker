using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Server
{
    public class ServerBuilderOptions : IBuilderOptions
    {
        public ServerBuilderOptions()
        {
            this.TcpMaxConnections = 100;
            this.UdpSocketObjectPoolSize = 10;
            this.PacketSizeBuffer = 5000;
            this.LogLevel = LogLevel.Error;
        }

        public int TcpPort { get; set; }
        public int UdpPort { get; set; }
        public int UdpSocketObjectPoolSize { get; set; }
        public int TcpMaxConnections { get; set; }
        public int PacketSizeBuffer { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}