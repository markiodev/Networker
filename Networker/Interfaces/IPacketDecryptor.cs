using Networker.Common;

namespace Networker.Interfaces
{
    public interface IPacketDecryptor
    {
        byte[] Decrypt(NetworkerPacketBase packetBase);
    }
}
