using System;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public class NoOpLogger : ILogger
    {
        public void Info(string message) { }
        public void Debug(string message) { }
        public void Error(Exception ex) { }
    }
}