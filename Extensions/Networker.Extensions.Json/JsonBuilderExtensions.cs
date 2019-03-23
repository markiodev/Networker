using Microsoft.Extensions.DependencyInjection;
using Networker.Client.Abstractions;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Extensions.Json
{
	public static class JsonBuilderExtensions
	{
		public static IServerBuilder UseJson(this IServerBuilder serverBuilder)
		{
			var serviceCollection = serverBuilder.GetServiceCollection();
			serviceCollection.AddSingleton<IPacketSerialiser, JsonSerialiser>();

			return serverBuilder;
		}

		public static IClientBuilder UseJson(this IClientBuilder clientBuilder)
		{
			var serviceCollection = clientBuilder.GetServiceCollection();
			serviceCollection.AddSingleton<IPacketSerialiser, JsonSerialiser>();
			return clientBuilder;
		}
	}
}