using MessagePack;
using System;

namespace Networker.Tests.MessagePack 
{
	[MessagePackObject]
    public class PingPacket
    {
		[Key(0)]
		public virtual DateTime Time { get; set; }

		public PingPacket(DateTime time) 
		{
			Time = time;
		}
    }
}
