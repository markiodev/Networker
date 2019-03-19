using System.Threading.Tasks;
using Networker.Common.Abstractions;

namespace Networker.Example.Json.PacketHandlers
{
	public class DynamicPacketHandlerBase : IPacketHandler
	{
		public async Task Handle(IPacketContext packetContext)
		{
			var packet = packetContext.Serialiser.Deserialise<object>(packetContext.PacketBytes);
			var method = GetType()
				.GetMethod("Process",
					new[] {packet.GetType(), typeof(IPacketContext)});
			await (Task) method.Invoke(this, new[] {packet, packetContext});
		}
	}
}