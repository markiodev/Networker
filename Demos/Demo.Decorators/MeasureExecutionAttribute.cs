using Networker.Common;
using System;

namespace Demo.Decorators
{
	public class MeasureExecutionAttribute : DecoratorAttribute 
	{
		public override Type[] Types => new Type[] { typeof(MeasureExecutionHandlerDecorator) };
	}
}
