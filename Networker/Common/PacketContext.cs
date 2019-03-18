using System;
using System.Collections.Generic;
using Networker.Common.Abstractions;

namespace Networker.Common
{
	public class PacketContext : IPacketContext
	{
		public Dictionary<string, object> Data { get; set; }
		public byte[] PacketBytes { get; set; }
		public string PacketName { get; set; }
		public IPacketHandler Handler { get; set; }
		public ISender Sender { get; set; }
		public IPacketSerialiser Serialiser { get; set; }

		public T GetData<T>(string name)
			where T: class
		{
			if(this.Data.ContainsKey(name))
			{
				return this.Data[name] as T;
			}

			return null;
		}

		public T GetPacket<T>()
			where T: class
		{
			return this.Serialiser.Deserialise<T>(this.PacketBytes);
		}
	}
}