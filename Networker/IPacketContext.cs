using System;

namespace Networker
{
    public interface IPacketContext
    {
        int ConnectionId { get; set; }
        IntPtr Packet { get; set; }
        byte[] PacketBytes { get; set; }
        int PacketLength { get; set; }
        IConnection Connection { get; set; }
    }
}