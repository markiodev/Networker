using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using Tutorial.Common;

namespace Tutorial.Client
{
	public class ClientChatPacketHandler : PacketHandlerBase<ChatPacket>
	{
		private readonly ILogger<ClientChatPacketHandler> _logger;

		public ClientChatPacketHandler(ILogger<ClientChatPacketHandler> logger)
		{
			_logger = logger;
		}

		public override async Task Process(ChatPacket packet, IPacketContext packetContext)
		{
			_logger.LogDebug("Client received the response packet: " + packet.Message);
		}
	}
}
