using System;
using Networker.Interfaces;

namespace Networker.NLog
{
    public class NetworkerNLogLoggerAdapter : INetworkerLoggerAdapter
    {
        public void Trace(string message) { }
    }
}