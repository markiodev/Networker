namespace Networker.V3.Server
{
    public interface IServer
    {
        void Lock();
        void Unlock();
        ITcpConnections GetConnections();
    }
}