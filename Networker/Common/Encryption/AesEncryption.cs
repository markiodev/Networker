using System;
using Networker.Interfaces;

namespace Networker.Common.Encryption
{
    public class AesEncryption : IPacketEncryption
    {
        public IPacketEncryptor GetEncryptor()
        {
            return null;
        }

        public IPacketDecryptor GetDecryptor()
        {
            return null;
        }

        public byte[] GenerateKey()
        {
            return new byte[] { };
        }

        public void SetKey(byte[] key) { }
    }
}