using System;
using SimpleNet.Common;
using ZeroFormatter;

namespace SimpleNet.Example
{
    public class ChatMessageReceivedPacket : SimpleNetPacketBase
    {
        [Index(0)]
        public virtual string Message { get; set; }
    }
}