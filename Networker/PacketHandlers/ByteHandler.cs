namespace Networker.PacketHandlers
{
    public abstract class ByteHandler : IPacketHandler
    {
        public void Handle(IPacketContext packetContext)
        {
        }

        public abstract void Handle(IPacketContext packetContext, byte[] bytes);
    }
}