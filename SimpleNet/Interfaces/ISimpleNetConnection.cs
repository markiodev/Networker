using System;
using SimpleNet.Common;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetConnection
    {
        ISimpleNetServerPacketReceipt CreatePacket(SimpleNetPacketBase packet);

        void Send<T>(T packet)
            where T: SimpleNetPacketBase;
    }
}