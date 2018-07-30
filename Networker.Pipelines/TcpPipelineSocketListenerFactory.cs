using System;
using Networker.Common.Abstractions;
using Networker.Server;
using Networker.Server.Abstractions;

namespace Networker.Pipelines
{
    public class TcpPipelineSocketListenerFactory : ITcpSocketListenerFactory
    {
        private readonly ILogger logger;
        private readonly ServerBuilderOptions options;
        private readonly IServerPacketProcessor serverPacketProcessor;

        public TcpPipelineSocketListenerFactory(ServerBuilderOptions options,
            ILogger logger,
            IServerPacketProcessor serverPacketProcessor)
        {
            this.options = options;
            this.logger = logger;
            this.serverPacketProcessor = serverPacketProcessor;
        }

        public ITcpSocketListener Create()
        {
            return new TcpPipelineSocketListener(this.options, this.logger, this.serverPacketProcessor);
        }
    }
}