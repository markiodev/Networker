using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class UdpClientListener : IUdpSocketListener
	{
		private readonly ObjectPool<EndPoint> endPointPool;
		private readonly ILogger logger;
		private readonly ServerBuilderOptions options;
		private readonly IServerInformation serverInformation;
		private readonly IServerPacketProcessor serverPacketProcessor;
		private UdpClient client;
		private IPEndPoint endPoint;

		public UdpClientListener(ServerBuilderOptions options,
			ILogger<UdpClientListener> logger,
			IServerPacketProcessor serverPacketProcessor,
			IServerInformation serverInformation)
		{
			this.options = options;
			this.logger = logger;
			this.serverPacketProcessor = serverPacketProcessor;
			this.serverInformation = serverInformation;

			endPointPool = new ObjectPool<EndPoint>(this.options.UdpSocketObjectPoolSize);

			for (var i = 0; i < endPointPool.Capacity; i++)
				endPointPool.Push(new IPEndPoint(IPAddress.Loopback, this.options.UdpPort));
		}

		public IPEndPoint GetEndPoint()
		{
			return endPoint;
		}

		public Socket GetSocket()
		{
			return client.Client;
		}

		public void Listen()
		{
			client = new UdpClient();
			client.ExclusiveAddressUse = false;
			client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			endPoint = new IPEndPoint(IPAddress.Loopback, options.UdpPort);
			client.Client.Bind(endPoint);

			logger.LogInformation($"Starting UDP listener on port {options.UdpPort}.");

			Task.Factory.StartNew(() =>
			{
				while (serverInformation.IsRunning)
				{
					var endpoint = endPointPool.Pop() as IPEndPoint;

					var receivedResults = client.Receive(ref endpoint);

					Process(endpoint, receivedResults);
				}
			});
		}

		private async Task Process(EndPoint endPoint, byte[] buffer)
		{
			serverPacketProcessor.ProcessUdpFromBuffer(endPoint, buffer);

			endPointPool.Push(endPoint);
		}
	}
}