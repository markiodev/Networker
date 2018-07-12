using System;
using Networker.Common;
using ZeroFormatter;

namespace Networker.DefaultPackets
{
    public class PingPacket : PacketBase
    {
        [Index(2)]
        public virtual DateTime Time{ get; set; }
    }
}