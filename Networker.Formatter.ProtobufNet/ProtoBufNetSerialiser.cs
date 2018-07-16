using System;
using System.IO;
using Networker.Common.Abstractions;
using ProtoBuf;

namespace Networker.Formatter.ProtobufNet
{
    public class ProtoBufNetSerialiser : IPacketSerialiser
    {
        public T Deserialise<T>(byte[] packetBytes)
        {
            using(var memoryStream = new MemoryStream(packetBytes))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }

        public byte[] Serialise<T>(T packet)
        {
            using(var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, packet);
                var packetBytes = memoryStream.ToArray();

                var packetWithData = new byte[packetBytes.Length + 4];

                var totalBytes = BitConverter.GetBytes(packetWithData.Length);

                for(var i = 0; i < totalBytes.Length; i++)
                {
                    packetWithData[i] = totalBytes[i];
                }

                Buffer.BlockCopy(packetBytes, 0, packetWithData, 4, packetBytes.Length);

                return packetWithData;
            }
        }
    }
}