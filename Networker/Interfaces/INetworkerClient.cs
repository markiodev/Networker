using System;
using Networker.Common;

namespace Networker.Interfaces
{
    public interface INetworkerClient
    {
        IContainerIoc Container { get; }
        INetworkerClient Connect();
        void Send(NetworkerPacketBase packet, NetworkerProtocol protocol = NetworkerProtocol.Tcp);

        IClientPacketReceipt SendAndHandleResponse<TResponseType>(NetworkerPacketBase packet,
            Action<TResponseType> handler)
            where TResponseType: class;

        IClientPacketReceipt SendAndHandleResponse(NetworkerPacketBase packet);

        IClientPacketReceipt SendAndHandleResponseAsync<TResponseType>(NetworkerPacketBase packet,
            Action<TResponseType> handler)
            where TResponseType: class;
    }
}