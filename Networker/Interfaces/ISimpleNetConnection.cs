using System;
using Networker.Common;

namespace Networker.Interfaces
{
    public interface INetworkerConnection
    {
        void Send<T>(T packet)
            where T: NetworkerPacketBase;
    }
}