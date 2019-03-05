using System.Net;

namespace Networker.Server.Abstractions
{
    public interface IUdpSocketSender
    {
        void SendTo(byte[] packetBytes, IPEndPoint endpoint);
        void SendTo<T>(T packet, IPEndPoint endpoint);
        void Broadcast<T>(T packet);
    }
}
