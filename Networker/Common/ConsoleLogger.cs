using System;
using Networker.Common.Abstractions;
using Networker.Server;

namespace Networker.Common
{
    public class ConsoleLogger : ILogger
    {
        private readonly ServerBuilderOptions options;

        public ConsoleLogger(ServerBuilderOptions options)
        {
            this.options = options;
            Console.WriteLine("WARNING: Using Console Logging can be detrimental to performance");
        }

        public void Debug(string message)
        {
            if(this.options.LogLevel == LogLevel.Debug)
                Console.WriteLine(message);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Info(string message)
        {
            if(this.options.LogLevel == LogLevel.Debug || this.options.LogLevel == LogLevel.Info)
                Console.WriteLine(message);
        }
    }
}