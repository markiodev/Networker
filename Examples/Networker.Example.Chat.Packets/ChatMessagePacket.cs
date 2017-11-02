using System;
using Networker.Common;
using ZeroFormatter;

namespace Networker.Example.Chat.Packets
{
    public class ChatMessagePacket : NetworkerPacketBase
    {
        [Index(0)]
        public virtual string Message { get; set; }

        [Index(1)]
        public virtual string Sender { get; set; }
    }
}
