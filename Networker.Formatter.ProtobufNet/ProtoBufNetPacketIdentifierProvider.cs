using System.IO;
using Networker.Common.Abstractions;
using ProtoBuf;

namespace Networker.Formatter.ProtobufNet
{
    public class ProtoBufNetPacketIdentifierProvider : IPacketIdentifierProvider
    {
        public string Provide(byte[] packet)
        {
            using(var memoryStream = new MemoryStream(packet))
            {
                var packetHeader = Serializer.Deserialize<ProtoBufPacketBase>(memoryStream);
                return packetHeader.UniqueKey;
            }
        }
    }
}