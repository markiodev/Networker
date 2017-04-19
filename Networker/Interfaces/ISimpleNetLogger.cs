using System;

namespace Networker.Interfaces
{
    public interface INetworkerLogger
    {
        void RegisterLogger(INetworkerLoggerAdapter loggerAdapter);
        void Trace(string message);
    }
}