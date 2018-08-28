using System;
using Microsoft.Extensions.DependencyInjection;
using Networker.Client.Abstractions;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.DefaultFormatter
{
    public static class DefaultNetworkerFormatterExtension
    {/*
        public static IServerBuilder UseDefaultFormatter(this IServerBuilder serverBuilder)
        {
            var serviceCollection = serverBuilder.GetServiceCollection();
            serviceCollection.AddSingleton<IPacketSerialiser, DefaultNetworkerPacketSerialiser>();

            return serverBuilder;
        }

        public static IClientBuilder UseDefaultFormatter(this IClientBuilder clientBuilder)
        {
            var serviceCollection = clientBuilder.GetServiceCollection();
            serviceCollection.AddSingleton<IPacketSerialiser, DefaultNetworkerPacketSerialiser>();
            return clientBuilder;
        }*/
    }
}