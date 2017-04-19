using System;

namespace Networker.Interfaces
{
    public interface IClientPacketReceipt
    {
        IClientPacketReceipt HandleResponse<T>(Action<T> responseHandler, int timeoutMsec = 30000);
        IClientPacketReceipt HandleResponseAsync<T>(Action<T> responseHandler);
        IClientPacketReceipt Send();
    }
}