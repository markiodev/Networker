namespace Networker.V3.Server
{
    public interface ITcpConnection
    {
        ITcpSocketListener SocketListener { get; }
    }
}