using System;
using Networker.Common;

namespace Networker.Server.Abstractions
{
    public interface IServer
    {
        void Start();
        void Stop();
        ITcpConnections GetConnections();
        EventHandler<ServerInformationEventArgs> ServerInformationUpdated { get; set; }
        EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }
        void Broadcast<T>(T packet);
    }
}