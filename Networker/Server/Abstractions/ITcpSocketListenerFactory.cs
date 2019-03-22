namespace Networker.Server.Abstractions
{
    public interface ITcpSocketListenerFactory
    {
        ITcpSocketListener Create();
    }
}
