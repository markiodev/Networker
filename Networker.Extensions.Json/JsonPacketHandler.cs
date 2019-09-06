namespace Networker.Extensions.Json
{
    public abstract class JsonPacketHandler<T> : IPacketHandler where T : class
    {
        public void Handle(IPacketContext packetContext)
        {
            HandlePacket(packetContext, null);
        }

        public abstract void HandlePacket(IPacketContext packetContext, T packet);
    }
}