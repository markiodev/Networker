namespace Networker
{
    public interface IPacketHandler
    {
        void Handle(IPacketContext packetContext);
    }
}