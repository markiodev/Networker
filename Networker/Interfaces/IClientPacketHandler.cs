using System;
using Networker.Common;

namespace Networker.Interfaces
{
    public interface IClientPacketHandler
    {
        void Handle(NetworkerPacketBase packet, byte[] bytes);
    }
}