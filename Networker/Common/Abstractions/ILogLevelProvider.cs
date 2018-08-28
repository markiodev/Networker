namespace Networker.Common.Abstractions
{
    public interface ILogLevelProvider
    {
        LogLevel GetLogLevel();
        void SetLogLevel(LogLevel logLevel);
    }
}