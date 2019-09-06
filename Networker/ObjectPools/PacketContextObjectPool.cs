using System.Runtime.InteropServices;

namespace Networker
{
    public class PacketContextObjectPool : ObjectPool<IPacketContext>
    {
        public PacketContextObjectPool(int capacity) : base(capacity)
        {
        }

        protected override void OnRelease(IPacketContext entity)
        {
            Marshal.FreeHGlobal(entity.Packet);
        }
    }
}