using System.Collections.Generic;

namespace Networker.Common.Abstractions
{
	public interface IPacketContext
	{
		Dictionary<string, object> Data { get; set; }
		IPacketHandler Handler { get; set; }
		byte[] PacketBytes { get; set; }
		string PacketName { get; set; }
		ISender Sender { get; set; }
		IPacketSerialiser Serialiser { get; set; }

		T GetData<T>(string name)
			where T : class;

		T GetPacket<T>()
			where T : class;
	}
}