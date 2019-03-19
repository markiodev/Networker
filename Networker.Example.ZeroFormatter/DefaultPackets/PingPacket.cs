using System;

namespace Networker.Example.ZeroFormatter.DefaultPackets
{
	[ZeroFormattable]
	public class PingPacket : ZeroFormatterPacketBase
	{
		[Index(2)] public virtual DateTime Time { get; set; }
	}
}