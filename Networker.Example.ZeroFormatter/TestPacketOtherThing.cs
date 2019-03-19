namespace Networker.Example.ZeroFormatter
{
	[ZeroFormattable]
	public class TestPacketOtherThing : ZeroFormatterPacketBase
	{
		[Index(2)] public virtual int SomeInt { get; set; }
	}
}