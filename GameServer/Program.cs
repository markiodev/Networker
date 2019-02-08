using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Networker.Formatter.ZeroFormatter;
using Networker.Server;

namespace GameServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var serverBuilder = new ServerBuilder()
                .UseTcp(1000)
                .UseUdp(1001)
                .UseConfiguration<Settings>(config)
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .RegisterTypes(serviceCollection =>
                {
                    serviceCollection.AddSingleton<IPlayerService, PlayerService>();
                })
                .RegisterPacketHandler<PlayerUpdatePacket, PlayerUpdatePacketHandler>()
                .UseZeroFormatter();

            var server = serverBuilder.Build();
            server.Start();
        }
    }
}