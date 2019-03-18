using System.Net.Sockets;

namespace Networker.Server.Abstractions
{
    public interface ISocketListener
    {
        Socket GetSocket();
        void Listen();
    }
}
