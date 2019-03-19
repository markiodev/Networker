using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using Tutorial.Common;

namespace Tutorial.Server
{
	public class ChatPacketHandler : PacketHandlerBase<ChatPacket>
	{
		private readonly ILogger<ChatPacketHandler> _logger;

		public ChatPacketHandler(ILogger<ChatPacketHandler> logger)
		{
			_logger = logger;
		}

		public override async Task Process(ChatPacket packet, IPacketContext packetContext)
		{
			_logger.LogDebug("I received the chat message: " + packet.Message);
			packetContext.Sender.Send(packet);
		}
	}
}