using System;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetLogger
    {
        void RegisterLogger(ISimpleNetLoggerAdapter loggerAdapter);
        void Trace(string message);
    }
}