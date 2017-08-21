using System;

namespace Networker.Interfaces
{
    public interface INetworkerLoggerAdapter
    {
        void Trace(string message);
        void Error(Exception exception);
    }
}