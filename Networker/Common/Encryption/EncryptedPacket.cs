using System;

namespace Networker.Common.Encryption
{
    public class EncryptedPacket : NetworkerPacketBase
    {
        public byte[] Data { get; set; }
    }
}