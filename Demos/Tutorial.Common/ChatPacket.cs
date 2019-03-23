using ProtoBuf;

namespace Tutorial.Common
{
	[ProtoContract]
	public class ChatPacket
	{
		[ProtoMember(1)] 
		public virtual string Name { get; set; }

		[ProtoMember(2)] 
		public virtual string Message { get; set; }
	}
}
