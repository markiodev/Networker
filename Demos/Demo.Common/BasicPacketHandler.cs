using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Demo.Common
{
	public class BasicPacketHandler : PacketHandlerBase<BasicPacket>
	{
		private readonly ILogger<BasicPacketHandler> logger;

		public BasicPacketHandler(ILogger<BasicPacketHandler> logger)
		{
			this.logger = logger;
		}

		public override async Task Process(BasicPacket packet, IPacketContext packetContext)
		{
			logger.LogDebug("Handling Basic Packet");
		}
	}
}