using System;
using System.IO;
using ZeroFormatter;

namespace SimpleNet.Common
{
    public class PacketSerializer
    {
        public byte[] Serialize<T>(T packet)
            where T: SimpleNetPacketBase
        {
            packet.UniqueKey = typeof(T).Name;

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