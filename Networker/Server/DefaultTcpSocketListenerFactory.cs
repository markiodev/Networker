using System;
using Microsoft.Extensions.Logging;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class DefaultTcpSocketListenerFactory : ITcpSocketListenerFactory
    {
        private readonly IBufferManager bufferManager;
        private readonly ITcpConnections connections;
        private readonly ILogger<TcpSocketListener> logger;
        private readonly IServerPacketProcessor serverPacketProcessor;
        private readonly ServerBuilderOptions options;

        public DefaultTcpSocketListenerFactory(IServerPacketProcessor serverPacketProcessor,
            IBufferManager bufferManager,
            ILogger<TcpSocketListener> logger,
            ITcpConnections connections,
            ServerBuilderOptions options)
        {
            this.serverPacketProcessor = serverPacketProcessor;
            this.bufferManager = bufferManager;
            this.logger = logger;
            this.connections = connections;
            this.options = options;
        }

        public ITcpSocketListener Create()
        {
            return new TcpSocketListener(this.options,
                this.serverPacketProcessor,
                this.bufferManager,
                this.logger,
                this.connections);
        }
    }
}