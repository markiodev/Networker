using System;
using SimpleNet.Common;

namespace SimpleNet.Interfaces
{
    public interface IClientPacketHandler
    {
        void Handle(SimpleNetPacketBase packet, byte[] bytes);
        Type GetPacketType();
    }
}