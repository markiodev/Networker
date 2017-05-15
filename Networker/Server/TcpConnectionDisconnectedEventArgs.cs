using System;

namespace Networker.Server
{
    public class TcpConnectionDisconnectedEventArgs : EventArgs
    {
        public TcpConnectionDisconnectedEventArgs(TcpConnection connection)
        {
            this.Connection = connection;
        }

        public TcpConnection Connection { get; }
    }
}