using System;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetClientPacketReceipt
    {
        ISimpleNetClientPacketReceipt HandleResponse<T>(Action<T> responseHandler, int timeoutMsec = 30000);
        ISimpleNetClientPacketReceipt HandleResponseAsync<T>(Action<T> responseHandler);
        ISimpleNetClientPacketReceipt Send();
    }
}