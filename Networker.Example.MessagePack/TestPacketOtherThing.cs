using MessagePack;

namespace Networker.Example.MessagePack 
{
	[MessagePackObject]
	public class TestPacketOtherThing
	{
		[Key(2)]
		public virtual int SomeInt { get; set; }
	}
}
