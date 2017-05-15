using System;

namespace Networker.Server
{
    public class TcpConnectionConnectedEventArgs : EventArgs
    {
        public TcpConnectionConnectedEventArgs(TcpConnection connection)
        {
            this.Connection = connection;
        }

        public TcpConnection Connection { get; }
    }
}