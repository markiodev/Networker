namespace SimpleNet.Example
{
    public class ChatMessageDispatchPacket : SimpleNetPacketBase
    {
        public string Sender { get; set; }
        public string Message { get; set; }

        public ChatMessageDispatchPacket(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }
    }
}