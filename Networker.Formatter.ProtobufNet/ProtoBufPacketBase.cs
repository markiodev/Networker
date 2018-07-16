using ProtoBuf;

namespace Networker.Formatter.ProtobufNet
{
    [ProtoContract]
    public class ProtoBufPacketBase
    {
        public ProtoBufPacketBase()
        {
            
        }

        [ProtoMember(1)]
        public string UniqueKey { get; set; }
    }
}