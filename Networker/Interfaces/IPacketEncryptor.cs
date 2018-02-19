using Networker.Common;

namespace Networker.Interfaces
{
    public interface IPacketEncryptor
    {
        byte[] Encrypt(NetworkerPacketBase packetBase);
    }
}
