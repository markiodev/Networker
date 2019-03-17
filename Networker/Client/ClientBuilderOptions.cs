using Microsoft.Extensions.Logging;
using Networker.Common.Abstractions;

namespace Networker.Client
{
    public class ClientBuilderOptions : IBuilderOptions
    {
        public int TcpPort { get; set; }
        public int UdpPort { get; set; }
        public string Ip { get; set; }
        public int UdpPortLocal { get; set; }
        public LogLevel LogLevel { get; set; }
        public int PacketSizeBuffer { get; set; }
        public int ObjectPoolSize { get; set; }

        public ClientBuilderOptions()
        {
            this.LogLevel = LogLevel.Error;
            this.PacketSizeBuffer = 3500;
            this.ObjectPoolSize = 200;
        }
    }
}