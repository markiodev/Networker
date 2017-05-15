using System;
using System.Collections.Generic;
using Networker.Common;
using Networker.Server;

namespace Networker.Interfaces
{
    public interface INetworkerServer
    {
        void Broadcast<T>(T packet)
            where T: NetworkerPacketBase;
        
        INetworkerServer Start();
        void Stop();

        EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }
        List<TcpConnection> Connections { get; }
    }
}