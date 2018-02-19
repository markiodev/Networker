namespace Networker.Common
{
    public interface IPacketSerializer
    {
        byte[] Serialize<T>(T packet) where T : NetworkerPacketBase;
    }
}