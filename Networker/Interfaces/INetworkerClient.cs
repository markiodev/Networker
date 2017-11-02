using System;
using Networker.Common;

namespace Networker.Interfaces
{
    public interface INetworkerClient
    {
        IContainerIoc Container { get; }
        INetworkerClient Connect();

        void Send<T>(T packet, NetworkerProtocol protocol = NetworkerProtocol.Tcp)
            where T: NetworkerPacketBase;

        IClientPacketReceipt SendAndHandleResponse<T, TResponseType>(T packet, Action<TResponseType> handler)
            where TResponseType: class where T: NetworkerPacketBase;

        /// <summary>
        /// Get the ping to server in milliseconds.
        /// </summary>
        /// <returns>Ping in milliseconds</returns>
        long Ping();

        /*IClientPacketReceipt SendAndHandleResponse(NetworkerPacketBase packet);

        IClientPacketReceipt SendAndHandleResponseAsync<TResponseType>(NetworkerPacketBase packet,
            Action<TResponseType> handler)
            where TResponseType: class;*/
    }
}