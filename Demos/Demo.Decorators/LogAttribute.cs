using Networker.Common;
using System;

namespace Demo.Decorators
{
	public class LogAttribute : DecoratorAttribute 
	{
		public override Type[] Types => new Type[] { typeof(LoggingHandlerDecorator) };
	}
}
