using System.Net.Sockets;

namespace Networker.Server.Abstractions
{
    public interface ITcpConnection
    {
        Socket Socket { get; set; }

        /// <summary>
        /// Store a custom object on this connection.
        /// </summary>
        object ConnectionIdentifier { get; set; }
    }
}
