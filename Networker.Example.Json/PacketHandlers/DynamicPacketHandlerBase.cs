using System;
using System.Reflection;
using System.Threading.Tasks;
using Networker.Common.Abstractions;

namespace Networker.Example.Json.PacketHandlers
{
	public class DynamicPacketHandlerBase : IPacketHandler
	{
		public async Task Handle(IPacketContext packetContext)
		{
			object packet = packetContext.Serialiser.Deserialise<object>(packetContext.PacketBytes);
			MethodInfo method = GetType().GetMethod("Process", new Type[] { packet.GetType(), typeof(IPacketContext) } );
			await (Task) method.Invoke(this, new object[] { packet, packetContext });
		}
	}
}