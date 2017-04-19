using System;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Helpers
{
    public static class ConsoleLoggerHelper
    {
        public static INetworkerServerBuilder UseConsoleLogger(this INetworkerServerBuilder builder)
        {
            builder.RegisterLogger(new ConsoleNetworkerLoggerAdapter("[{0} Server]"));
            return builder;
        }

        public static INetworkerClientBuilder UseConsoleLogger(this INetworkerClientBuilder builder)
        {
            builder.RegisterLogger(new ConsoleNetworkerLoggerAdapter("[{0} Client]"));
            return builder;
        }
    }
}