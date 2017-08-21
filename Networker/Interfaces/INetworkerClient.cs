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

        IClientPacketReceipt SendAndHandleResponse<T, TResponseType>(T packet,
            Action<TResponseType> handler)
            where TResponseType: class where T : NetworkerPacketBase;

        /*IClientPacketReceipt SendAndHandleResponse(NetworkerPacketBase packet);

        IClientPacketReceipt SendAndHandleResponseAsync<TResponseType>(NetworkerPacketBase packet,
            Action<TResponseType> handler)
            where TResponseType: class;*/
    }
}