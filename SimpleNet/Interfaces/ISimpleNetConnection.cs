using System;
using System.Net.Sockets;
using SimpleNet.Common;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetConnection
    {
        Socket Socket { get; }
        ISimpleNetServerPacketReceipt CreatePacket(SimpleNetPacketBase packet);

        void Send<T>(T packet)
            where T: SimpleNetPacketBase;
    }
}