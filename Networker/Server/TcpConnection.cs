using Networker.Server.Abstractions;
using System.Net.Sockets;

namespace Networker.Server
{
    public class TcpConnection : ITcpConnection
    {
        public Socket Socket { get; set; }

        public TcpConnection(Socket socket)
        {
            this.Socket = socket;
        }
    }
}
