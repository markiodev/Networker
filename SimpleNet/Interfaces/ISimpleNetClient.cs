using System;
using SimpleNet.Common;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetClient
    {
        ISimpleNetClient Connect();
        ISimpleNetClientPacketReceipt CreatePacket(SimpleNetPacketBase packet);

        void Send<T>(T packet, SimpleNetProtocol protocol = SimpleNetProtocol.Tcp)
            where T: SimpleNetPacketBase;
    }
}