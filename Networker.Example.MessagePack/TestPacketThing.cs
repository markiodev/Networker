using MessagePack;

namespace Networker.Example.MessagePack 
{
	[MessagePackObject]
	public class TestPacketThing
	{
		[Key(2)]
		public virtual string SomeString { get; set; }
	}
}
