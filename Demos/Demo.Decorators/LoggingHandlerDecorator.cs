using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using System.Threading.Tasks;

namespace Demo.Decorators 
{
	public class LoggingHandlerDecorator : PacketHandlerDecorator<SomePacket>
    {
		private readonly ILogger logger;

        public LoggingHandlerDecorator(ILogger logger)
        {
            this.logger = logger;
        }

        public override async Task Process(SomePacket packet, IPacketContext context)
        {
			logger.LogInformation("Logging to fake database: SomePacket received, SomePacket.SomeString = " + packet.SomeString);
			await ContinueProcessing(packet, context);
        }
    }
}