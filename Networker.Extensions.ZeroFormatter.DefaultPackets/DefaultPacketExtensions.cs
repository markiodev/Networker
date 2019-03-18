using System;
using Networker.Client.Abstractions;

namespace Networker.Extensions.ZeroFormatter.DefaultPackets
{
	public static class DefaultPacketExtensions
	{
		public static void Send(this IClient client, byte[] bytes)
		{
			client.Send(new BytePacket(bytes));
		}

		public static void Send(this IClient client, int integerValue) { }
	}
}