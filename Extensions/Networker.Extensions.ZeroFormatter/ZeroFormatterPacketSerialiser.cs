using System;
using System.IO;
using System.Text;
using Networker.Common.Abstractions;
using ZeroFormatter;

namespace Networker.Extensions.ZeroFormatter
{
	public class ZeroFormatterPacketSerialiser : IPacketSerialiser
	{
		public bool CanReadLength => true;
		public bool CanReadName => true;

		public bool CanReadOffset => false;

		public T Deserialise<T>(byte[] packetBytes)
		{
			return ZeroFormatterSerializer.Deserialize<T>(packetBytes);
		}

		public T Deserialise<T>(byte[] packetBytes, int offset, int length)
		{
			throw new NotImplementedException();
		}

		public byte[] Package(string name, byte[] bytes)
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var binaryWriter = new BinaryWriter(memoryStream))
				{
					var nameBytes = Encoding.ASCII.GetBytes(name);
					binaryWriter.Write(nameBytes.Length);
					binaryWriter.Write(bytes.Length);
					binaryWriter.Write(nameBytes);
					binaryWriter.Write(bytes);
				}

				var packetBytes = memoryStream.ToArray();
				return packetBytes;
			}
		}

		public byte[] Serialise<T>(T packet)
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var binaryWriter = new BinaryWriter(memoryStream))
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
	}
}