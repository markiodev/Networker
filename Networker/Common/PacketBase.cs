using System;
using ZeroFormatter;

namespace Networker.Common
{
    [ZeroFormattable]
    public abstract class PacketBase
    {
        public PacketBase()
        {
            
        }
        
        [Index(1)]
        public virtual string UniqueKey { get; set; }
    }
}