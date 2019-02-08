using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace GameServer
{
    public class PlayerUpdatePacketHandler : PacketHandlerBase<PlayerUpdatePacket>
    {
        private readonly ILogger<PlayerUpdatePacketHandler> logger;

        public PlayerUpdatePacketHandler(ILogger<PlayerUpdatePacketHandler> logger)
        {
            this.logger = logger;
        }

        public override async Task Process(PlayerUpdatePacket packet, IPacketContext context)
        {
            this.logger.LogInformation("Wow some logging!");
        }
    }
}