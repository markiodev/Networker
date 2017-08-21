using System;
using System.Collections.Generic;
using Networker.Interfaces;

namespace Networker.Client
{
    public class ClientResponseStore
    {
        private readonly Dictionary<string, IClientPacketReceipt> receipts;

        public ClientResponseStore()
        {
            this.receipts = new Dictionary<string, IClientPacketReceipt>();
        }

        public IClientPacketReceipt Find(string deserializedTransactionId)
        {
            lock(this.receipts)
            {
                if(this.receipts.ContainsKey(deserializedTransactionId))
                {
                    return this.receipts[deserializedTransactionId];
                }
            }

            return null;
        }

        public void Remove(string packetBaseTransactionId)
        {
            lock(this.receipts)
                this.receipts.Remove(packetBaseTransactionId);
        }

        public void Store(string packetBaseTransactionId, IClientPacketReceipt receipt)
        {
            lock(this.receipts)
                this.receipts.Add(packetBaseTransactionId, receipt);
        }
    }
}