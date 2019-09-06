using System;

namespace Networker
{
    public class PacketContext : IPacketContext
    {
        public int ConnectionId { get; set; }
        public IntPtr Packet { get; set; }
        public byte[] PacketBytes { get; set; }
        public int PacketLength { get; set; }
        public IConnection Connection { get; set; }
    }
}