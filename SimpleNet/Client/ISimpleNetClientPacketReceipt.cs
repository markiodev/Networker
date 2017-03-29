using System;

namespace SimpleNet.Client
{
    public interface ISimpleNetClientPacketReceipt
    {
        ISimpleNetClientPacketReceipt HandleResponsePacket<T>(Action<T> responseHandler, int timeoutMsec = 30000);
        ISimpleNetClientPacketReceipt HandleResponsePacketAsync<T>(Action<T> responseHandler);
    }
}