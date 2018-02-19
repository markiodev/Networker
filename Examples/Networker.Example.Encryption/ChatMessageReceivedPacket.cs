using System;
using Networker.Common;
using ZeroFormatter;

namespace Networker.Example.Encryption
{
    public class ChatMessageReceivedPacket : NetworkerPacketBase
    {
        [Index(0)]
        public virtual string Message { get; set; }
    }
}