using System;
using System.Collections.Generic;

namespace Networker.Common.Abstractions
{
	public interface IPacketHandlerModule
	{
		Dictionary<Type, Type> GetPacketHandlers();
	}
}