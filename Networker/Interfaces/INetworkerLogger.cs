using System;

namespace Networker.Interfaces
{
    public interface INetworkerLogger
    {
        void Error(Exception exception);
        void RegisterLogger(INetworkerLoggerAdapter loggerAdapter);
        void Trace(string message);
    }
}