using System.Net;

namespace Networker.Server.Abstractions
{
    public interface IUdpSocketListener : ISocketListener
    {
        IPEndPoint GetEndPoint();
    }
}
