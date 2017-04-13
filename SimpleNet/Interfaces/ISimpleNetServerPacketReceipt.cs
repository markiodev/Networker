using System;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetServerPacketReceipt
    {
        void Encrypt();
        void Send();
    }
}