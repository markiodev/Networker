using Microsoft.Extensions.DependencyInjection;
using Networker.Client.Abstractions;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Extensions.ProtobufNet
{
	public static class ProtoBufNetBuilderExtension
	{
		public static IServerBuilder UseProtobufNet(this IServerBuilder serverBuilder)
		{
			var serviceCollection = serverBuilder.GetServiceCollection();
			serviceCollection.AddSingleton<IPacketSerialiser, ProtoBufNetSerialiser>();

			return serverBuilder;
		}

		public static IClientBuilder UseProtobufNet(this IClientBuilder clientBuilder)
		{
			var serviceCollection = clientBuilder.GetServiceCollection();
			serviceCollection.AddSingleton<IPacketSerialiser, ProtoBufNetSerialiser>();
			return clientBuilder;
		}
	}
}