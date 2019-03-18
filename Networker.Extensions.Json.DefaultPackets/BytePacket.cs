using System;

namespace Networker.Extensions.Json.DefaultPackets
{
	public class BytePacket
	{
		public BytePacket(byte[] bytes)
		{
			this.Bytes = bytes;
		}

		public byte[] Bytes { get; set; }
	}
}
