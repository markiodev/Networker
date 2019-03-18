using System;
using System.Collections.Generic;

namespace Networker.Common.Abstractions
{
    public interface IPacketContext
    {
        ISender Sender { get; set; }
        byte[] PacketBytes { get; set; }
        T GetPacket<T>() where T: class;
		Dictionary<string, object> Data { get; set; }
		IPacketSerialiser Serialiser { get; set; }
		string PacketName { get; set; }
		IPacketHandler Handler { get; set; }
		T GetData<T>(string name) where T: class;
    }
}