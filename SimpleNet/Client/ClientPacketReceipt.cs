using System;
using SimpleNet.Interfaces;

namespace SimpleNet.Client
{
    public class ClientPacketReceipt : IClientPacketReceipt
    {
        public IClientPacketReceipt HandleResponse<T>(Action<T> responseHandler,
            int timeoutMsec = 30000)
        {
            throw new NotImplementedException();
        }

        public IClientPacketReceipt HandleResponseAsync<T>(Action<T> responseHandler)
        {
            throw new NotImplementedException();
        }

        public IClientPacketReceipt Send()
        {
            throw new NotImplementedException();
        }
    }
}