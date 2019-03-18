using Networker.Server.Abstractions;
using System.Net.Sockets;

namespace Networker.Server
{
    public class TcpConnection : ITcpConnection
    {
        public Socket Socket { get; set; }

        public object UserTag { get; set; }

        public TcpConnection(Socket socket, object userTag = null)
        {
            this.Socket = socket;
            this.UserTag = userTag;
        }
    }
}
