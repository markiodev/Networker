using MessagePack;
using Networker.Common;
using Networker.Common.Abstractions;
using System;
using System.IO;
using System.Text;

namespace Networker.Formatter.MessagePack 
{
	public class MessagePackPacketSerialiser : IPacketSerialiser 
	{

		private readonly ObjectPool<MemoryStream> memoryStreamObjectPool;

		public MessagePackPacketSerialiser()
        {
            this.memoryStreamObjectPool = new ObjectPool<MemoryStream>(1500);

            for(var i = 0; i < this.memoryStreamObjectPool.Capacity; i++)
            {
                this.memoryStreamObjectPool.Push(new MemoryStream());
            }
        }

        public byte[] Package(string name, byte[] bytes)
        {
            return new byte[] { };
        }

		public bool CanReadOffset => true;
        public bool CanReadName => true;
        public bool CanReadLength => true;

		public T Deserialise<T>(byte[] packetBytes)
		{
			var deserialized = MessagePackSerializer.Typeless.Deserialize(packetBytes);

			return (T) deserialized;
		}

		public T Deserialise<T>(byte[] packetBytes, int offset, int length) 
		{
			var memoryStream = this.memoryStreamObjectPool.Pop();

            memoryStream.SetLength(0);
            memoryStream.Write(packetBytes, offset, length);

            var deserialised = MessagePackSerializer.Typeless.Deserialize(memoryStream);

            this.memoryStreamObjectPool.Push(memoryStream);

            return (T) deserialised;
		}

		public byte[] Serialise<T>(T packet) 
		{
			var packetBytes = MessagePackSerializer.Typeless.Serialize(packet);
			
			var name = packet.GetType().Name;
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
