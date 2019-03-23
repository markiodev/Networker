using System;
using System.Collections.Generic;
using Networker.Common.Abstractions;

namespace Networker.Common
{
	public abstract class PacketHandlerModuleBase : IPacketHandlerModule
	{
		private readonly Dictionary<Type, Type> _packetHandlers = new Dictionary<Type, Type>();

		public Dictionary<Type, Type> GetPacketHandlers()
		{
			return _packetHandlers;
		}

		public void AddPacketHandler<TPacket, TPacketHandler>()
		{
			_packetHandlers.Add(typeof(TPacket), typeof(TPacketHandler));
		}
	}
}