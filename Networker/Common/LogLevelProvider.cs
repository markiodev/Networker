using Networker.Common.Abstractions;

namespace Networker.Common
{
    public class LogLevelProvider : ILogLevelProvider
    {
        private LogLevel _logLevel;

        public LogLevel GetLogLevel()
        {
            return _logLevel;
        }

        public void SetLogLevel(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }
    }
}