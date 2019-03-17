using MessagePack;
using System;

namespace Networker.Example.MessagePack.DefaultPackets
{
    [MessagePackObject]
    public class PingPacket
    {
        [Key(2)]
        public virtual DateTime Time { get; set; }
    }
}