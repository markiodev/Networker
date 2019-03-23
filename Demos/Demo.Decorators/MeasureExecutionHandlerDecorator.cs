using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Demo.Decorators
{
    public class MeasureExecutionHandlerDecorator : PacketHandlerDecorator<SomePacket>
    {
		private readonly ILogger logger;

        public MeasureExecutionHandlerDecorator(ILogger logger)
        {
            this.logger = logger;
        }

        public override async Task Process(SomePacket packet, IPacketContext context)
        {
			Stopwatch stopwatch = Stopwatch.StartNew();
			await ContinueProcessing(packet, context);
			stopwatch.Stop();
			long elapsedMs = stopwatch.ElapsedMilliseconds;
			logger.LogInformation("SomePacket took " + elapsedMs + " to process.");
        }
    }
}