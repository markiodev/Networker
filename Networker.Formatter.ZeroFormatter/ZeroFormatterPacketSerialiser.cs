using System;
using System.IO;
using Networker.Common;
using Networker.Common.Abstractions;
using ZeroFormatter;

namespace Networker.Formatter.ZeroFormatter
{
    public class ZeroFormatterPacketSerialiser : IPacketSerialiser
    {
        public T Deserialise<T>(byte[] packetBytes)
        {
            return ZeroFormatterSerializer.Deserialize<T>(packetBytes);
        }

        public byte[] Serialise<T>(T packet)
        {
            var zeroFormatterPacket = packet as ZeroFormatterPacketBase;

            if(zeroFormatterPacket != null)
            {
                zeroFormatterPacket.UniqueKey = packet.GetType()
                                                      .Name;
            }
            else
            {
                throw new Exception("Packet must derive from ZeroFormatterPacketBase");
            }

            using(var memoryStream = new MemoryStream())
            {
                using(var binaryWriter = new BinaryWriter(memoryStream))
                {
                    var serialised = ZeroFormatterSerializer.Serialize(packet);
                    binaryWriter.Write(serialised.Length);
                    binaryWriter.Write(serialised);
                }

                var packetBytes = memoryStream.ToArray();
                return packetBytes;
            }
        }
    }
}