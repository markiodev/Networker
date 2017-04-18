using System;
using SimpleNet.Common;
using SimpleNet.Server;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetServer
    {
        void Broadcast<T>(T packet)
            where T: SimpleNetPacketBase;

        void Send<T>(ISimpleNetConnection connection,
            T packet,
            SimpleNetProtocol protocol = SimpleNetProtocol.Tcp)
            where T: SimpleNetPacketBase;

        ISimpleNetServer Start();
        void Stop();

        EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }
    }
}