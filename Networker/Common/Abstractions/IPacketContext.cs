namespace Networker.Common.Abstractions
{
    public interface IPacketContext
    {
        ISender Sender { get; set; }
        byte[] PacketBytes { get; set; }
    }
}