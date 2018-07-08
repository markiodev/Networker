namespace Networker.V3.Server
{
    public class Server : IServer
    {
        public void Lock() { }
        public void Unlock() { }

        public ITcpConnections GetConnections()
        {
            return null;
        }
    }
}