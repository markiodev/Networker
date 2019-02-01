using System;
using Microsoft.Extensions.Logging;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class DefaultUdpSocketListenerFactory : IUdpSocketListenerFactory
    {
        private readonly ILogger<UdpClientListener> logger;
        private readonly ServerBuilderOptions options;
        private readonly IServerPacketProcessor serverPacketProcessor;
        private IServerInformation serverInformation;

        public DefaultUdpSocketListenerFactory(ServerBuilderOptions options,
            ILogger<UdpClientListener> logger,
            IServerPacketProcessor serverPacketProcessor,
            IServerInformation serverInformation)
        {
            this.options = options;
            this.logger = logger;
            this.serverPacketProcessor = serverPacketProcessor;
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
                this.serverInformation);
        }
    }
}