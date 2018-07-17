using System;
using System.IO;
using System.Text;
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

        public T Deserialise<T>(byte[] packetBytes, int offset, int length)
        {
            return default(T);
        }

        public byte[] Serialise<T>(T packet)
        {
            using(var memoryStream = new MemoryStream())
            {
                using(var binaryWriter = new BinaryWriter(memoryStream))
                {
                    var nameBytes = Encoding.ASCII.GetBytes(typeof(T).Name);
                    var serialised = ZeroFormatterSerializer.Serialize(packet);
                    binaryWriter.Write(nameBytes.Length);
                    binaryWriter.Write(serialised.Length);
                    binaryWriter.Write(nameBytes);
                    binaryWriter.Write(serialised);
                }

                var packetBytes = memoryStream.ToArray();
                return packetBytes;
            }
        }

        public bool CanReadOffset => false;
    }
}