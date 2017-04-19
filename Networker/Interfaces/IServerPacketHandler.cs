using System;
using Networker.Common;

namespace Networker.Interfaces
{
    public interface IServerPacketHandler
    {
        void Handle(INetworkerConnection clientConnection, NetworkerPacketBase packet, byte[] bytes);
    }
}