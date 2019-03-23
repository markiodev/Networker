using System;
using Microsoft.Extensions.Logging;

namespace Networker.Common
{
	public static class LoggingExtensions
	{
		public static void Error(this ILogger logger, Exception exception)
		{
			logger.Log(LogLevel.Error, exception.ToString());
		}
	}
}