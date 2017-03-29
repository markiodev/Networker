using SimpleNet.Client;

namespace SimpleNet
{
    public interface ISimpleNetClient
    {
        ISimpleNetClient Connect();
        ISimpleNetClientPacketReceipt Send(ISimpleNetPacket packet);
    }
}