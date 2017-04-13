using SimpleNet.Common;
using SimpleNet.Interfaces;

namespace SimpleNet.NLog
{
    public static class SimpleNetNLogBuilderHelper
    {
        public static ISimpleNetServerBuilder UseConsoleLogger(this ISimpleNetServerBuilder builder)
        {
            builder.RegisterLogger(new SimpleNetNLogLoggerAdapter());
            return builder;
        }

        public static ISimpleNetClientBuilder UseConsoleLogger(this ISimpleNetClientBuilder builder)
        {
            builder.RegisterLogger(new SimpleNetNLogLoggerAdapter());
            return builder;
        }
    }
}