using Microsoft.Extensions.DependencyInjection;
using Networker.Client.Abstractions;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Client
{
	public class ClientBuilder : BuilderBase<IClientBuilder, IClient, ClientBuilderOptions>, IClientBuilder
	{
		public override IClient Build()
		{
			SetupSharedDependencies();
			serviceCollection.AddSingleton<IClient, Client>();
			serviceCollection.AddSingleton<IClientPacketProcessor, ClientPacketProcessor>();
			serviceCollection.AddSingleton<IUdpSocketSender, UdpClientSender>();

			var serviceProvider = GetServiceProvider();
			return serviceProvider.GetService<IClient>();
		}

		public T Build<T>()
			where T : IClient
		{
			SetupSharedDependencies();
			serviceCollection.AddSingleton(typeof(T), typeof(Client));
			serviceCollection.AddSingleton<IClientPacketProcessor, ClientPacketProcessor>();
			serviceCollection.AddSingleton<IUdpSocketSender, UdpClientSender>();

			var serviceProvider = GetServiceProvider();
			return serviceProvider.GetService<T>();
		}

		public IClientBuilder SetPacketBufferPoolSize(int size)
		{
			options.ObjectPoolSize = size;
			return this;
		}

		public IClientBuilder UseIp(string ip)
		{
			if (ip == "localhost") ip = "127.0.0.1";

			options.Ip = ip;
			return this;
		}

		public IClientBuilder UseUdp(int port, int localPort)
		{
			options.UdpPort = port;
			options.UdpPortLocal = localPort;
			return this;
		}
	}
}