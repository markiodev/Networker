using Networker.Server.Abstractions;
using System.Net.Sockets;

namespace Networker.Server
{
    public class TcpConnection : ITcpConnection
    {
        public Socket Socket { get; set; }

        public object ConnectionIdentifier { get; set; }

        public TcpConnection(Socket socket, object connectionIdentifier = null)
        {
            this.Socket = socket;
            this.ConnectionIdentifier = connectionIdentifier;
        }
    }
}
