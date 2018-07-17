using System;
using System.IO;
using System.Text;
using Networker.Common;
using Networker.Common.Abstractions;
using ProtoBuf;

namespace Networker.Formatter.ProtobufNet
{
    public class ProtoBufNetSerialiser : IPacketSerialiser
    {
        private readonly ObjectPool<MemoryStream> memoryStreamObjectPool;

        public ProtoBufNetSerialiser()
        {
            this.memoryStreamObjectPool = new ObjectPool<MemoryStream>(1500);

            for(var i = 0; i < this.memoryStreamObjectPool.Capacity; i++)
            {
                this.memoryStreamObjectPool.Push(new MemoryStream());
            }
        }

        public bool CanReadOffset => true;

        public T Deserialise<T>(byte[] packetBytes, int offset, int length)
        {
            var memoryStream = this.memoryStreamObjectPool.Pop();

            memoryStream.SetLength(0);
            memoryStream.Write(packetBytes, offset, length);

            var deserialised = Serializer.Deserialize<T>(memoryStream);

            this.memoryStreamObjectPool.Push(memoryStream);

            return deserialised;
        }

        public byte[] Serialise<T>(T packet)
        {
            var memoryStream = this.memoryStreamObjectPool.Pop();

            memoryStream.SetLength(0);

            Serializer.Serialize(memoryStream, packet);

            var packetBytes = memoryStream.ToArray();

            this.memoryStreamObjectPool.Push(memoryStream);

            var name = typeof(T).Name;
            var nameBytes = Encoding.ASCII.GetBytes(name);

            var packetWithData = new byte[packetBytes.Length + 8 + nameBytes.Length];

            var packetCountBytes = BitConverter.GetBytes(packetBytes.Length);
            var packetNameCountBytes = BitConverter.GetBytes(nameBytes.Length);

            int currentPosition = 0;

            foreach(var nameByte in packetNameCountBytes)
            {
                packetWithData[currentPosition++] = nameByte;
            }

            foreach(var packetByte in packetCountBytes)
            {
                packetWithData[currentPosition++] = packetByte;
            }

            Buffer.BlockCopy(nameBytes, 0, packetWithData, currentPosition, nameBytes.Length);

            currentPosition += nameBytes.Length;

            Buffer.BlockCopy(packetBytes, 0, packetWithData, currentPosition, packetBytes.Length);

            return packetWithData;
        }
    }
}