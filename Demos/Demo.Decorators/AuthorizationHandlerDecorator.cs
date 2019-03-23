using Networker.Common;
using Networker.Common.Abstractions;
using System.Reflection;
using System.Threading.Tasks;

namespace Demo.Decorators 
{
	// TODO: Test and make sure works with inheritance and multiple methods
	// Aka replace SomePacket with PacketBase and test on SomePacket and SomeOtherPacket
	public class AuthorizationHandlerDecorator : PacketHandlerDecorator<SomePacket>
    {
        public override async Task Process(SomePacket packet, IPacketContext context)
        {
			Role userRole = Role.AuthenticatedUser; // Get from service
			Role minimumRequiredRole = GetMinimumRequiredRole(packet);
			if (userRole >= minimumRequiredRole) 
			{
				await ContinueProcessing(packet, context);
			}
        }

		private Role GetMinimumRequiredRole(SomePacket packet) 
		{
			RequireRoleAttribute authorize = GetProcessMethodInfo(packet).GetCustomAttribute<RequireRoleAttribute>();
			if (authorize == null) 
			{
				return Role.GuestUser; // No authorization required
			} 
			else
			{
				return authorize.Role;
			}
		}
    }
}