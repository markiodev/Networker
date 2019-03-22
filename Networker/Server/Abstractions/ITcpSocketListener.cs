using System;

namespace Networker.Server.Abstractions
{
    public interface ITcpSocketListener : ISocketListener
    {
        EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }
    }
}
