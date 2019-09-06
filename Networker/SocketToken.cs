using System.Net.Sockets;

namespace Networker
{
    public class SocketToken
    {
        public Socket Socket { get; set; }
        public int Id { get; set; }
        public TcpConnection Connection { get; set; }
    }
}