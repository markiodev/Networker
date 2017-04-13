using System;
using SimpleNet.Interfaces;

namespace SimpleNet.NLog
{
    public class SimpleNetNLogLoggerAdapter : ISimpleNetLoggerAdapter
    {
        public void Trace(string message) { }
    }
}