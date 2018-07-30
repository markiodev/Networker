using System;
using Networker.Formatter.ProtobufNet;
using ProtoBuf;

namespace Networker.Example.ProtoBuf
{
    [ProtoContract]
    public class PingPacket : ProtoBufPacketBase
    {
        [ProtoMember(2)]
        public virtual DateTime Time { get; set; }
    }
}