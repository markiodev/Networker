using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using System.Threading.Tasks;

namespace Networker.Example.MessagePack 
{
	public class TestPacketHandler : 
		PacketHandlerBase,
		IPacketHandler<TestPacketThing>,
		IPacketHandler<TestPacketOtherThing> 
	{
		private readonly ILogger logger;

        public TestPacketHandler(ILogger<TestPacketHandler> logger)
        {
            this.logger = logger;
        }

		public Task Process(TestPacketThing packet, IPacketContext context)
		{
			this.logger.LogDebug("Received a thing packet from " + context.Sender.EndPoint);
			this.logger.LogDebug("Payload: " + packet.SomeString);
			return Task.CompletedTask;
		}

		public Task Process(TestPacketOtherThing packet, IPacketContext context) 
		{
			this.logger.LogDebug("Received an other thing packet from " + context.Sender.EndPoint);
			this.logger.LogDebug("Payload: " + packet.SomeInt);
			return Task.CompletedTask;
		}
	}
}
