using Networker.Common;
using System;

namespace Demo.Decorators
{
	public class AuthorizeAttribute : DecoratorAttribute 
	{
		public override Type[] Types => new Type[] { typeof(AuthorizationHandlerDecorator) };
	}
}
