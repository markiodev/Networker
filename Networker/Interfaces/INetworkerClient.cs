using System;
using Networker.Common;

namespace Networker.Interfaces
{
    public interface INetworkerClient
    {
        INetworkerClient Connect();
        IClientPacketReceipt CreatePacket(NetworkerPacketBase packet);

        void Send<T>(T packet, NetworkerProtocol protocol = NetworkerProtocol.Tcp)
            where T: NetworkerPacketBase;
    }
}