using System;

namespace SimpleNet.Example
{
    public class ChatMessageDispatchPacketHandler : ISimpleNetServerPacketHandler<ChatMessageDispatchPacket>
    {
        public void Handle(ChatMessageDispatchPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}