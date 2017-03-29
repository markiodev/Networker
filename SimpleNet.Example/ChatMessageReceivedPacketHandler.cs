namespace SimpleNet.Example
{
    public class ChatMessageReceivedPacketHandler : ISimpleNetClientPacketHandler<ChatMessageReceivedPacket>
    {
        public void Handle(ISimpleNetConnection connection, ChatMessageReceivedPacket packet)
        {
            throw new System.NotImplementedException();
        }
    }
}