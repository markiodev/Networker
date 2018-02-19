
namespace Networker.Interfaces
{
    public interface IPacketEncryption
    {
        IPacketEncryptor GetEncryptor();
        IPacketDecryptor GetDecryptor();
        byte[] GenerateKey();
        void SetKey(byte[] key);
    }
}
