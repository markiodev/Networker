using System;
using SimpleNet.Common;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetClientPacketHandler
    {
        void Handle(SimpleNetPacketBase packet, byte[] bytes);
        Type GetPacketType();
    }
}