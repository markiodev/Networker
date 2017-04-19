using Networker.Common;
using Networker.Interfaces;

namespace Networker.NLog
{
    public static class NetworkerNLogBuilderHelper
    {
        public static INetworkerServerBuilder UseConsoleLogger(this INetworkerServerBuilder builder)
        {
            builder.RegisterLogger(new NetworkerNLogLoggerAdapter());
            return builder;
        }

        public static INetworkerClientBuilder UseConsoleLogger(this INetworkerClientBuilder builder)
        {
            builder.RegisterLogger(new NetworkerNLogLoggerAdapter());
            return builder;
        }
    }
}