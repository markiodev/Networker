using System;
using Networker.Interfaces;

namespace Networker.Common
{
    public class ConsoleNetworkerLoggerAdapter : INetworkerLoggerAdapter
    {
        private readonly string _prefix;

        public ConsoleNetworkerLoggerAdapter(string prefix)
        {
            this._prefix = prefix;
        }

        public void Trace(string message)
        {
            Console.WriteLine($"{string.Format(this._prefix, DateTime.Now)} {message}");
        }
    }
}