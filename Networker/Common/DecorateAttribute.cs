using System;

namespace Networker.Common 
{
	public class DecorateAttribute : DecoratorAttribute 
	{
		public override Type[] Types => types;

		private readonly Type[] types;

		public DecorateAttribute(params Type[] types) 
		{
			this.types = types;
		}
	}
}
