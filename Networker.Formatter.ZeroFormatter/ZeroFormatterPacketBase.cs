using System;
using ZeroFormatter;

namespace Networker.Formatter.ZeroFormatter
{
    [ZeroFormattable]
    public abstract class ZeroFormatterPacketBase
    {
        public ZeroFormatterPacketBase()
        {
        }

        [Index(1)]
        public virtual string UniqueKey { get; set; }
    }
}