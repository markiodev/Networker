using System.Net;

namespace Networker.Common.Abstractions
{
    public interface ISender
    {
        IPEndPoint EndPoint { get; }

        /// <summary>
        /// Send a TCP packet to the client.
        /// </summary>
        /// <typeparam name="T">The packet type.</typeparam>
        /// <param name="packet">The packet.</param>
        void Send<T>(T packet);
    }
}
