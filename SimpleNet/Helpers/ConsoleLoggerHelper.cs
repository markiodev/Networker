using System;
using SimpleNet.Common;
using SimpleNet.Interfaces;

namespace SimpleNet.Helpers
{
    public static class ConsoleLoggerHelper
    {
        public static ISimpleNetServerBuilder UseConsoleLogger(this ISimpleNetServerBuilder builder)
        {
            builder.RegisterLogger(new ConsoleSimpleNetLoggerAdapter("[{0} Server]"));
            return builder;
        }

        public static ISimpleNetClientBuilder UseConsoleLogger(this ISimpleNetClientBuilder builder)
        {
            builder.RegisterLogger(new ConsoleSimpleNetLoggerAdapter("[{0} Client]"));
            return builder;
        }
    }
}