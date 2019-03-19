using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Example.ZeroFormatter.DefaultPackets
{
	public class PingPacketHandler : PacketHandlerBase<PingPacket>
	{
		private readonly ILogger logger;

		public PingPacketHandler(ILogger<PingPacketHandler> logger)
		{
			this.logger = logger;
		}

		public override async Task Process(PingPacket packet, IPacketContext context)
		{
			logger.LogDebug("Received a ping packet from " + context.Sender.EndPoint);
		}
	}
}