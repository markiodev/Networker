using System;

namespace Networker.Common.Abstractions
{
    public interface ISender
    {
        void Send(PacketBase packet);
    }
}