using Microsoft.Extensions.DependencyInjection;
using Networker.Client.Abstractions;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Extensions.ZeroFormatter
{
	public static class ZeroFormatterBuilderExtension
	{
		public static IServerBuilder UseZeroFormatter(this IServerBuilder serverBuilder)
		{
			var serviceCollection = serverBuilder.GetServiceCollection();
			serviceCollection.AddSingleton<IPacketSerialiser, ZeroFormatterPacketSerialiser>();

			return serverBuilder;
		}

		public static IClientBuilder UseZeroFormatter(this IClientBuilder clientBuilder)
		{
			var serviceCollection = clientBuilder.GetServiceCollection();
			serviceCollection.AddSingleton<IPacketSerialiser, ZeroFormatterPacketSerialiser>();
			return clientBuilder;
		}
	}
}