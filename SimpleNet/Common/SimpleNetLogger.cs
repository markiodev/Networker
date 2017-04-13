using System;
using System.Collections.Generic;
using SimpleNet.Interfaces;

namespace SimpleNet.Common
{
    public class SimpleNetLogger : ISimpleNetLogger
    {
        private readonly List<ISimpleNetLoggerAdapter> logAdapters;

        public SimpleNetLogger()
        {
            this.logAdapters = new List<ISimpleNetLoggerAdapter>();
        }

        public void RegisterLogger(ISimpleNetLoggerAdapter loggerAdapter)
        {
            this.logAdapters.Add(loggerAdapter);
        }

        public void Trace(string message)
        {
            foreach(var logAdapter in this.logAdapters)
            {
                logAdapter.Trace(message);
            }
        }
    }
}