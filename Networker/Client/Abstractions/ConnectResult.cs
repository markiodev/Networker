using System.Collections.Generic;
using System.Linq;

namespace Networker.Client.Abstractions
{
	public class ConnectResult
	{
		public ConnectResult(bool success)
		{
			Success = success;
			Errors = null;
		}

		public ConnectResult(IEnumerable<string> errors)
		{
			Success = errors.Count() == 0;
			Errors = errors;
		}

		public ConnectResult(params string[] errors)
		{
			Success = errors.Count() == 0;
			Errors = errors;
		}

		public IEnumerable<string> Errors { get; }
		public bool Success { get; }
	}
}