using System;
using System.Net.Sockets;

namespace Networker.Client.Abstractions
{
    public interface IClient
    {
        /// <summary>
        /// Occurs when the client is connected.
        /// </summary>
        EventHandler<Socket> Connected { get; set; }

        /// <summary>
        /// Occurs when the client is disconnected.
        /// </summary>
        EventHandler<Socket> Disconnected { get; set; }

        /// <summary>
        /// Serialize and send a Tcp packet.
        /// </summary>
        /// <typeparam name="T">The passed type.</typeparam>
        /// <param name="packet">The packet.</param>
        void Send<T>(T packet);

        /// <summary>
        /// Send a raw byte array via Tcp packet.
        /// </summary>
        /// <remarks>
        /// Keep in mind that Networker Server require the packet header,
        /// which is handled by the <see cref="Common.Abstractions.IPacketSerialiser"/>.
        ///
        /// So always recommended to use <see cref="Send{T}(T)"/> instead.
        /// </remarks>
        /// <param name="packet">The packet byte array.</param>
        void Send(byte[] packet);

        /// <summary>
        /// Serialize and send a Udp packet.
        /// </summary>
        /// <typeparam name="T">The passed type.</typeparam>
        /// <param name="packet">The packet.</param>
        void SendUdp<T>(T packet);

        /// <summary>
        /// Send a raw byte array via Udp packet.
        /// </summary>
        /// <remarks>
        /// Keep in mind that Networker Server require the packet header,
        /// which is handled by the <see cref="Common.Abstractions.IPacketSerialiser"/>.
        ///
        /// So always recommended to use <see cref="SendUdp{T}(T)"/> instead.
        /// </remarks>
        /// <param name="packet">The packet byte array.</param>
        void SendUdp(byte[] packet);

        /// <summary>
        /// Send a ping to the server.
        /// </summary>
        /// <param name="timeout">The amount of time until timout.</param>
        /// <returns>Returns -1 if unsuccessful; Otherwise the roundtrip time.</returns>
        long Ping(int timeout = 10000);

        /// <summary>
        /// Connect to the Server.
        /// </summary>
        /// <returns></returns>
        ConnectResult Connect();

        /// <summary>
        /// Close the connection to the server.
        /// </summary>
        void Stop();
    }
}
