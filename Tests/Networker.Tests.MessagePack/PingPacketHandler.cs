using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using System.Threading.Tasks;

namespace Networker.Tests.MessagePack 
{
	public class PingPacketHandler : PacketHandlerBase<PingPacket>
    {
        private readonly ILogger logger;

        public PingPacketHandler(ILogger<PingPacketHandler> logger, IPacketSerialiser serialiser)
        {
            this.logger = logger;
        }

		public override Task Process(PingPacket packet, IPacketContext context) 
		{
			logger.LogDebug("Received a ping packet from " + context.Sender.EndPoint + " at " + packet.Time);
			return Task.CompletedTask;
		}
	}
}
