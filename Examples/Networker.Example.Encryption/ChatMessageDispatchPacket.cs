using System;
using Networker.Common;
using ZeroFormatter;

namespace Networker.Example.Encryption.Packets
{
    public class ChatMessageDispatchPacket : NetworkerPacketBase
    {
        [Index(1)]
        public virtual string Message { get; set; }
        [Index(0)]
        public virtual string Sender { get; set; }
    }
}