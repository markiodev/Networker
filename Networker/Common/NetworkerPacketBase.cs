using System;
using ZeroFormatter;

namespace Networker.Common
{
    [ZeroFormattable]
    public abstract class NetworkerPacketBase
    {
        public NetworkerPacketBase()
        {
            
        }

        [Index(97)]
        public virtual string TransactionId { get; set; }

        [Index(98)]
        public virtual string UniqueKey { get; set; }
    }
}