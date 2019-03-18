using System;
using System.Threading.Tasks;
using Networker.Common.Abstractions;
using Networker.Example.Json.Middleware;

namespace Networker.Example.Json
{
	[RoleRequired(RoleName = "Admin")]
	public class TestPacketOneHandler : IPacketHandler
	{
		public async Task Handle(IPacketContext packetContext)
		{
			if(packetContext.PacketName == "PacketType1")
			{

			}
			else if(packetContext.PacketName == "PacketType2")
			{

			}
		}
	}
}