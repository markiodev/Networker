using System;

namespace Networker.Interfaces
{
    public interface INetworkerLoggerAdapter
    {
        void Error(Exception exception);
        void Trace(string message);
    }
}