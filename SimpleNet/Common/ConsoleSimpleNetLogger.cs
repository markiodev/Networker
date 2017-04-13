using System;
using SimpleNet.Interfaces;

namespace SimpleNet.Common
{
    public class ConsoleSimpleNetLoggerAdapter : ISimpleNetLoggerAdapter
    {
        private readonly string _prefix;

        public ConsoleSimpleNetLoggerAdapter(string prefix)
        {
            this._prefix = prefix;
        }

        public void Trace(string message)
        {
            Console.WriteLine($"{string.Format(this._prefix, DateTime.Now)} {message}");
        }
    }
}