using System;
using System.IO;
using System.Text;
using Networker.Common;
using Networker.Common.Abstractions;
using ProtoBuf;

namespace Networker.Extensions.ProtobufNet
{
	public class ProtoBufNetSerialiser : IPacketSerialiser
	{
		private readonly ObjectPool<MemoryStream> memoryStreamObjectPool;

		public ProtoBufNetSerialiser()
		{
			memoryStreamObjectPool = new ObjectPool<MemoryStream>(1500);

			for (var i = 0; i < memoryStreamObjectPool.Capacity; i++) memoryStreamObjectPool.Push(new MemoryStream());
		}

		public bool CanReadLength => true;
		public bool CanReadName => true;

		public bool CanReadOffset => false; //Was TRUE

		public T Deserialise<T>(byte[] packetBytes)
		{
			/*var memoryStream = memoryStreamObjectPool.Pop();

			memoryStream.SetLength(0);
			memoryStream.Write(packetBytes, 0, packetBytes.Length);

			var deserialised = Serializer.Deserialize<T>(memoryStream);

			memoryStreamObjectPool.Push(memoryStream);*/
			//todo: FIX for perf
			
			return Serializer.Deserialize<T>(new MemoryStream(packetBytes));
		}

		public T Deserialise<T>(byte[] packetBytes, int offset, int length)
		{
			throw new NotSupportedException();
		}

		public byte[] Package(string name, byte[] bytes)
		{
			return new byte[] { };
		}

		public byte[] Serialise<T>(T packet)
		{
			var memoryStream = memoryStreamObjectPool.Pop();

			memoryStream.SetLength(0);

			Serializer.Serialize(memoryStream, packet);

			var packetBytes = memoryStream.ToArray();
			
			var name = typeof(T).Name;
			var nameBytes = Encoding.ASCII.GetBytes(name);

			var packetWithData = new byte[packetBytes.Length + 8 + nameBytes.Length];

			var packetCountBytes = BitConverter.GetBytes(packetBytes.Length);
			var packetNameCountBytes = BitConverter.GetBytes(nameBytes.Length);

			var currentPosition = 0;

			foreach (var nameByte in packetNameCountBytes) packetWithData[currentPosition++] = nameByte;

			foreach (var packetByte in packetCountBytes) packetWithData[currentPosition++] = packetByte;

			Buffer.BlockCopy(nameBytes, 0, packetWithData, currentPosition, nameBytes.Length);

			currentPosition += nameBytes.Length;

			Buffer.BlockCopy(packetBytes, 0, packetWithData, currentPosition, packetBytes.Length);

			memoryStreamObjectPool.Push(memoryStream);
			
			var testDeserialise = Serializer.Deserialize<T>(new MemoryStream(packetBytes));
			return packetWithData;
		}
	}
}