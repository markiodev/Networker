using System;

namespace Demo.Decorators
{
	public class RequireRoleAttribute : Attribute 
	{
		public Role Role { get; private set; }

		public RequireRoleAttribute(Role role) 
		{
			Role = role;
		}
	}
}
