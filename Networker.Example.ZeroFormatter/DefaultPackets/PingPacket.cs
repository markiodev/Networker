using System;
using Networker.Formatter.ZeroFormatter;
using ZeroFormatter;

namespace Networker.Example.ZeroFormatter.DefaultPackets
{
    [ZeroFormattable]
    public class PingPacket : ZeroFormatterPacketBase
    {
        [Index(2)]
        public virtual DateTime Time { get; set; }
    }
}