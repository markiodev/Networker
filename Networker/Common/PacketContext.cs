using Networker.Common.Abstractions;

namespace Networker.Common
{
    public class PacketContext : IPacketContext
    {
        public ISender Sender { get; set; }
        public byte[] PacketBytes { get; set; }
    }
}