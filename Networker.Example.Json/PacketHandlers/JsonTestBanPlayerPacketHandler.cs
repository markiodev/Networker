using Microsoft.Extensions.Logging;
using Networker.Example.Json.Middleware;
using Networker.Example.Json.Packets;

namespace Networker.Example.Json.PacketHandlers
{
	[RoleRequired(RoleName = "Admin")]
	public class JsonTestBanPlayerPacketHandler : JsonTestPacketHandler<JsonTestBanPlayerPacket>
	{
		public JsonTestBanPlayerPacketHandler(ILogger<JsonTestPacketHandler<JsonTestBanPlayerPacket>> logger)
			: base(logger)
		{
		}
	}
}