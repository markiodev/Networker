using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Example.Json.Packets;

namespace Networker.Example.Json.PacketHandlers
{
	public class JsonTestPacketHandler<T> : PacketHandlerBase<T> where T : JsonTestPacket
	{
		private readonly ILogger<JsonTestPacketHandler<T>> logger;

		public JsonTestPacketHandler(ILogger<JsonTestPacketHandler<T>> logger)
		{
			this.logger = logger;
		}

		public override async Task Process(T packet, IPacketContext packetContext)
		{
			var jsonTestPacketChild = packet as JsonTestPacketChild;

			if(jsonTestPacketChild != null)
			{
				//this is a JSON child packet
				this.logger.LogDebug("This is a CHILD");
			}
			else
			{
				this.logger.LogDebug("This is a PARENT");
			}
		}
	}
}