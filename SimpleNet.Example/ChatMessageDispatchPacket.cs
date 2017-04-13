using System;
using SimpleNet.Common;
using ZeroFormatter;

namespace SimpleNet.Example
{
    [ZeroFormattable]
    public class ChatMessageDispatchPacket : SimpleNetPacketBase
    {
        [Index(1)]
        public virtual string Message { get; set; }
        [Index(0)]
        public virtual string Sender { get; set; }
    }
}