using System;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class TcpConnectionDisconnectedEventArgs : EventArgs
    {
        public TcpConnectionDisconnectedEventArgs(ITcpConnection connection)
        {
            this.Connection = connection;
        }

        public ITcpConnection Connection { get; }
    }
}