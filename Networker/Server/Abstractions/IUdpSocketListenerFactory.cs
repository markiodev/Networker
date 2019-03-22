namespace Networker.Server.Abstractions
{
    public interface IUdpSocketListenerFactory
    {
        IUdpSocketListener Create();
    }
}
