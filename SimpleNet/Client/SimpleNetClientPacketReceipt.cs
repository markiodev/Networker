using System;
using SimpleNet.Interfaces;

namespace SimpleNet.Client
{
    public class SimpleNetClientPacketReceipt : ISimpleNetClientPacketReceipt
    {
        public ISimpleNetClientPacketReceipt HandleResponse<T>(Action<T> responseHandler,
            int timeoutMsec = 30000)
        {
            throw new NotImplementedException();
        }

        public ISimpleNetClientPacketReceipt HandleResponseAsync<T>(Action<T> responseHandler)
        {
            throw new NotImplementedException();
        }

        public ISimpleNetClientPacketReceipt Send()
        {
            throw new NotImplementedException();
        }
    }
}