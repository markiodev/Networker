using Networker;

namespace Demo.ChatCommon
{
    public class MessageOfTheDay : PacketBase
    {
        public override int PacketTypeId => (int)PacketIdentifiers.MessageOfTheDay;
        public string Message { get; set; }
        public string SetBy { get; set; }
    }
}