using System;
using System.Collections.Generic;
using Networker.Interfaces;

namespace Networker.Common
{
    public class NetworkerLogger : INetworkerLogger
    {
        private readonly List<INetworkerLoggerAdapter> logAdapters;

        public NetworkerLogger()
        {
            this.logAdapters = new List<INetworkerLoggerAdapter>();
        }

        public void Error(Exception exception)
        {
            foreach(var logAdapter in this.logAdapters)
            {
                logAdapter.Error(exception);
            }
        }

        public void RegisterLogger(INetworkerLoggerAdapter loggerAdapter)
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