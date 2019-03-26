using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Networker.Server.Abstractions
{
    public interface ITcpConnections
    {
        /// <summary>
        /// Gets all the current TCP connections.
        /// </summary>
        /// <returns></returns>
        List<ITcpConnection> GetConnections();
        
        /// <summary>
        /// Broadcast a TCP packet to all connections.
        /// </summary>
        /// <typeparam name="T">The packet type.</typeparam>
        /// <param name="packet">The packet.</param>
        void Broadcast<T>(T packet);

        /// <summary>
        /// Finds a connection by endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>Connection if found; otherwise,, null.</returns>
        ITcpConnection FindByEndpoint(EndPoint endpoint);

        ITcpConnection Add(Socket connection);
        void Remove(ITcpConnection connection);
    }
}
