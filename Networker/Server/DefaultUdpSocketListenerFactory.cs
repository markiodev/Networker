using System;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class DefaultUdpSocketListenerFactory : IUdpSocketListenerFactory
    {
        private readonly IBufferManager bufferManager;
        private readonly ILogger logger;
        private readonly ServerBuilderOptions options;
        private readonly IServerPacketProcessor serverPacketProcessor;
        private IServerInformation serverInformation;

        public DefaultUdpSocketListenerFactory(ServerBuilderOptions options,
            ILogger logger,
            IServerPacketProcessor serverPacketProcessor,
            IBufferManager bufferManager,
            IServerInformation serverInformation)
        {
            this.options = options;
            this.logger = logger;
            this.serverPacketProcessor = serverPacketProcessor;
            this.bufferManager = bufferManager;
            this.serverInformation = serverInformation;
        }

        public IUdpSocketListener Create()
        {
            /*return new UdpSocketListener(this.options,
                this.logger,
                this.serverPacketProcessor,
                this.bufferManager);*/

            return new UdpClientListener(this.options,
                this.logger,
                this.serverPacketProcessor,
                this.bufferManager,
                this.serverInformation);
        }
    }
}