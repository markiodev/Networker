using System;
using System.Net.Sockets;
using Networker.Server.Abstractions;

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