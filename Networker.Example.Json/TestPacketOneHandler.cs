using System;
using System.Threading.Tasks;
using Networker.Common.Abstractions;
using Networker.Example.ZeroFormatter.Middleware;

namespace Networker.Example.ZeroFormatter
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

		//todo: REMOVE THESE
		public async Task Handle(byte[] packet, IPacketContext packetContext)
		{
		}

		public Task Handle(byte[] packet, int offset, int length, IPacketContext packetContext)
		{
			return null;
		}
	}
}