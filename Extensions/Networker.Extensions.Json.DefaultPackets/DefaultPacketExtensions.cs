using Networker.Client.Abstractions;

namespace Networker.Extensions.Json.DefaultPackets
{
	public static class DefaultPacketExtensions
	{
		public static void Send(this IClient client, byte[] bytes)
		{
			client.Send(new BytePacket(bytes));
		}
	}
}