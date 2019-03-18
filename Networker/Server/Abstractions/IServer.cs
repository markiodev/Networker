using System;

namespace Networker.Server.Abstractions
{
    public interface IServer
    {
        IServerInformation Information { get; }

        ITcpConnections GetConnections();

        EventHandler<ServerInformationEventArgs> ServerInformationUpdated { get; set; }
        EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }

        /// <summary>
        /// Broadcast a UDP Packet to all clients.
        /// </summary>
        /// <typeparam name="T">The packet type.</typeparam>
        /// <param name="packet">The packet.</param>
        void Broadcast<T>(T packet);

        void Start();
        void Stop();
    }
}
