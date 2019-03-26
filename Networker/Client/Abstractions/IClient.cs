using System;
using System.Net.Sockets;

namespace Networker.Client.Abstractions
{
    public interface IClient
    {
        EventHandler<Socket> Connected { get; set; }
        EventHandler<Socket> Disconnected { get; set; }

        /// <summary>
        /// Send a TCP packet to the server.
        /// </summary>
        /// <typeparam name="T">The packet type.</typeparam>
        /// <param name="packet">The packet.</param>
        void Send<T>(T packet);

        void SendUdp<T>(T packet);
        void SendUdp(byte[] packet);

        long Ping(int timeout = 10000);

        ConnectResult Connect();

        void Stop();
    }
}
