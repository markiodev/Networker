using System;
using System.Collections.Generic;
using Networker.Common;
using Networker.Server;

namespace Networker.Interfaces
{
    public interface INetworkerServer
    {
        List<TcpConnection> Connections { get; }

        EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }

        void Broadcast<T>(T packet)
            where T: NetworkerPacketBase;

        INetworkerServer Start();
        void Stop();

        IContainerIoc GetIocContainer();
    }
}