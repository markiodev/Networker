using System;

namespace Networker.Example.ZeroFormatter.Middleware
{
	public class RoleRequired : Attribute
	{
		public string RoleName { get; set; }
	}
}