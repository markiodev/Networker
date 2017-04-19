using System;
using Networker.Common;
using Networker.Server;

namespace Networker.Interfaces
{
    public interface INetworkerServer
    {
        void Broadcast<T>(T packet)
            where T: NetworkerPacketBase;

        void Send<T>(INetworkerConnection connection,
            T packet,
            NetworkerProtocol protocol = NetworkerProtocol.Tcp)
            where T: NetworkerPacketBase;

        INetworkerServer Start();
        void Stop();

        EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }
    }
}