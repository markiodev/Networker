using System;
using Networker.Common;
using Networker.Interfaces;
using ZeroFormatter;

namespace Networker.Client
{
    public class ClientPacketReceipt<T> : IClientPacketReceipt
        where T: class
    {
        private readonly INetworkerClient client;
        private readonly NetworkerPacketBase packetBase;

        public ClientPacketReceipt(INetworkerClient client, NetworkerPacketBase packetBase)
        {
            this.client = client;
            this.packetBase = packetBase;
        }

        public Action<T> ResponseHandler { get; set; }

        public int Timeout { get; set; }

        public IClientPacketReceipt HandleResponse(Action<T> responseHandler, int timeoutMsec = 30000)
        {
            this.ResponseHandler = responseHandler;
            this.Timeout = timeoutMsec;

            return this;
        }

        public void Invoke(byte[] packet)
        {
            this.ResponseHandler.Invoke(ZeroFormatterSerializer.Deserialize<T>(packet));
        }

        public IClientPacketReceipt Send()
        {
            this.client.Send(this.packetBase);
            return this;
        }
    }
}