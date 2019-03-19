using Networker.Common;

namespace Networker.Example.ZeroFormatter
{
	public class ExamplePacketHandlerModule : PacketHandlerModuleBase
	{
		public ExamplePacketHandlerModule()
		{
			AddPacketHandler<TestPacketThing, TestPacketOneHandler>();
			AddPacketHandler<TestPacketOtherThing, TestPacketTwoHandler>();
		}
	}
}