using System;

namespace Networker.V3.Common
{
    public interface ILogger
    {
        void Info(string message);
        void Debug(string message);
        void Error(Exception ex);
    }
}