using System;

namespace Networker.Common.Abstractions
{
    public interface ILogger
    {
        void Info(string message);
        void Debug(string message);
        void Error(Exception ex);
    }
}