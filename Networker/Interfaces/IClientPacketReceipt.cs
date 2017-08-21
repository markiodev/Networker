using System;

namespace Networker.Interfaces
{
    public interface IClientPacketReceipt
    {
        void Invoke(byte[] packet);
        IClientPacketReceipt Send();
    }
}