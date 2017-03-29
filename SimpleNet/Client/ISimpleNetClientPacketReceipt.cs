using System;

namespace SimpleNet.Client
{
    public interface ISimpleNetClientPacketReceipt
    {
        ISimpleNetClientPacketReceipt HandleResponsePacket(Action<ISimpleNetPacket> responseHandler, int timeoutMsec = 30000);
        ISimpleNetClientPacketReceipt HandleResponsePacketAsync(Action<ISimpleNetPacket> responseHandler);
    }
}