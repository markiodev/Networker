
using Microsoft.Extensions.Logging;

namespace Networker.Common.Abstractions
{
    public interface IBuilderOptions
    {
        int TcpPort { get; set; }
        int UdpPort { get; set; }
        int PacketSizeBuffer { get; set; }
        LogLevel LogLevel { get; set; }
    }
}
