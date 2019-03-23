using Microsoft.Extensions.Logging;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class DefaultUdpSocketListenerFactory : IUdpSocketListenerFactory
	{
		private readonly ILogger<UdpClientListener> logger;
		private readonly ServerBuilderOptions options;
		private readonly IServerInformation serverInformation;
		private readonly IServerPacketProcessor serverPacketProcessor;
		private IUdpSocketListener _clientListener;

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

			return _clientListener ?? (_clientListener = new UdpClientListener(options,
				       logger,
				       serverPacketProcessor,
				       serverInformation));
		}
	}
}