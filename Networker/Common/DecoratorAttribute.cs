using System;

namespace Networker.Common 
{
	public abstract class DecoratorAttribute : Attribute
	{
		public abstract Type[] Types { get; }
	}
}