using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Networker.Common.Abstractions;

namespace Networker.DefaultFormatter
{
	public class DefaultNetworkerPacketSerialiser : IPacketSerialiser
	{
		public bool CanReadLength => true;
		public bool CanReadName => true;

		public bool CanReadOffset => false;

		public T Deserialise<T>(byte[] packetBytes)
		{
			throw new NotImplementedException();
		}

		public T Deserialise<T>(byte[] packetBytes, int offset, int length)
		{
			throw new NotImplementedException();
			return (T) Activator.CreateInstance(typeof(T), packetBytes);
		}

		public byte[] Package(string name, byte[] bytes)
		{
			throw new NotImplementedException();
		}

		public byte[] Serialise<T>(T packet)
		{
			throw new NotImplementedException();
			var bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, packet);
				return ms.ToArray();
			}
		}
	}
}