using Networker.Common;

namespace Networker.Interfaces
{
    public interface IPacketEncryptor
    {
        byte[] Encrypt(byte[] packetBase);
    }
}
