using System;
using Networker.Common;
using ZeroFormatter;

namespace Networker.Example
{
    public class ChatMessageReceivedPacket : NetworkerPacketBase
    {
        [Index(0)]
        public virtual string Message { get; set; }
    }
}