using System.Collections.Generic;
using Networker.Common.Abstractions;

namespace Networker.Common
{
	public class PacketHandlers : IPacketHandlers
	{
		private readonly Dictionary<string, IPacketHandler> packetHandlers;

		public PacketHandlers()
		{
			packetHandlers = new Dictionary<string, IPacketHandler>();
		}

		public void Add(string name, IPacketHandler packetHandler)
		{
			packetHandlers.Add(name, packetHandler);
		}

		public Dictionary<string, IPacketHandler> GetPacketHandlers()
		{
			return packetHandlers;
		}
	}
}