using System.Net.Sockets;

namespace Networker.Server.Abstractions
{
    public interface ITcpConnection
    {
        Socket Socket { get; set; }
    }
}
