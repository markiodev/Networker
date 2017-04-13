using System;
using ZeroFormatter;

namespace SimpleNet.Common
{
    [ZeroFormattable]
    public abstract class SimpleNetPacketBase
    {
        public SimpleNetPacketBase()
        {
            
        }

        [Index(98)]
        public virtual string UniqueKey { get; set; }
    }
}