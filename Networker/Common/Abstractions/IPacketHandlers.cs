using System.Collections.Generic;

namespace Networker.Common.Abstractions
{
	public interface IPacketHandlers
	{
		void Add(string name, IPacketHandler packetHandler);
		Dictionary<string, IPacketHandler> GetPacketHandlers();
	}
}