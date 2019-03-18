using System;

namespace Networker.Example.Json.Middleware
{
	public class RoleRequired : Attribute
	{
		public string RoleName { get; set; }
	}
}