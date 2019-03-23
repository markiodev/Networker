using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Demo.Decorators
{
	// Build in attribute
	[Decorate(
		typeof(AuthorizationHandlerDecorator),
		typeof(LoggingHandlerDecorator),
		typeof(MeasureExecutionHandlerDecorator))]

	// -- or --

	// Custom attributes
	[Authorize, Log, MeasureExecution]
    public class SomePacketHandler : PacketHandlerBase<SomePacket>
    {
        private readonly ILogger logger;

        public SomePacketHandler(ILogger logger)
        {
            this.logger = logger;
        }
        
		[RequireRole(Role.Administrator)] 
        public override Task Process(SomePacket packet, IPacketContext context)
        {
			Thread.Sleep(1000); // To test measure execution time decorator
            logger.LogInformation("Processing SomePacket.");
			return Task.CompletedTask;
        }

		public string GetLog(SomePacket packet) 
		{
			return "Packet received with data: " + packet.SomeString;
		}
    }
}