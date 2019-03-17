using System.Collections.Generic;
using System.Linq;

namespace Networker.Client.Abstractions
{
    public class ConnectResult
    {
        public bool Success { get; }

        public IEnumerable<string> Errors { get; }

        public ConnectResult(bool success)
        {
            this.Success = success;
            this.Errors = null;
        }

        public ConnectResult(IEnumerable<string> errors)
        {
            this.Success = errors.Count() == 0;
            this.Errors = errors;
        }

        public ConnectResult(params string[] errors)
        {
            this.Success = errors.Count() == 0;
            this.Errors = errors;
        }
    }
}
