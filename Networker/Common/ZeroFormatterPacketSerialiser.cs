using System;
using System.IO;
using Networker.Common.Abstractions;
using ZeroFormatter;

namespace Networker.Common
{
    public class ZeroFormatterPacketSerialiser : IPacketSerialiser
    {
        public T Deserialise<T>(byte[] packetBytes)
        {
            return ZeroFormatterSerializer.Deserialize<T>(packetBytes);
        }

        public byte[] Serialise(PacketBase packet)
        {
            packet.UniqueKey = packet.GetType()
                                     .Name;

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