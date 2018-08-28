using System;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public class ConsoleLogger : ILogger
    {
        private readonly ILogLevelProvider _logLevelProvider;

        public ConsoleLogger(ILogLevelProvider logLevelProvider)
        {
            _logLevelProvider = logLevelProvider;
            Console.WriteLine("WARNING: Using Console Logging can be detrimental to performance");
        }

        public void Debug(string message)
        {
            if (_logLevelProvider.GetLogLevel() == LogLevel.Debug)
                Console.WriteLine(message);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Info(string message)
        {
            if (_logLevelProvider.GetLogLevel() == LogLevel.Debug || _logLevelProvider.GetLogLevel() == LogLevel.Info)
                Console.WriteLine(message);
        }
    }
}