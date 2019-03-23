using Networker.Common.Abstractions;

namespace Networker.Common
{
	public class PacketSerialiserProvider
	{
		public static IPacketSerialiser PacketSerialiser { get; set; }

		public static IPacketSerialiser Provide()
		{
			return PacketSerialiser;
		}
	}
}