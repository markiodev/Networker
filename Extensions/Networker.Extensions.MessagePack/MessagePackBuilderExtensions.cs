using Microsoft.Extensions.DependencyInjection;
using Networker.Client.Abstractions;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Extensions.MessagePack
{
	public static class MessagePackBuilderExtensions
	{
		public static IServerBuilder UseMessagePack(this IServerBuilder serverBuilder)
		{
			var serviceCollection = serverBuilder.GetServiceCollection();
			serviceCollection.AddSingleton<IPacketSerialiser, MessagePackPacketSerialiser>();
			return serverBuilder;
		}

		public static IClientBuilder UseMessagePack(this IClientBuilder clientBuilder)
		{
			var serviceCollection = clientBuilder.GetServiceCollection();
			serviceCollection.AddSingleton<IPacketSerialiser, MessagePackPacketSerialiser>();
			return clientBuilder;
		}
	}
}