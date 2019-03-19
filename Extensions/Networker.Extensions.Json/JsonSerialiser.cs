using System.IO;
using System.Text;
using Networker.Common.Abstractions;
using Utf8Json;

namespace Networker.Extensions.Json
{
	public class JsonSerialiser : IPacketSerialiser
	{
		public bool CanReadLength => true;
		public bool CanReadName => true;

		public bool CanReadOffset => false;

		public T Deserialise<T>(byte[] packetBytes)
		{
			return JsonSerializer.Deserialize<T>(packetBytes);
		}

		public T Deserialise<T>(byte[] packetBytes, int offset, int length)
		{
			return default(T);
		}

		public byte[] Package(string name, byte[] bytes)
		{
			return new byte[] { };
		}

		public byte[] Serialise<T>(T packet)
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var binaryWriter = new BinaryWriter(memoryStream))
				{
					var nameBytes = Encoding.ASCII.GetBytes(typeof(T).Name);
					var serialised = JsonSerializer.Serialize(packet);
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