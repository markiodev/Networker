using Networker.Common.Abstractions;

namespace Networker.Client.Abstractions
{
    public interface IClientBuilder : IBuilder<IClientBuilder, IClient>
    {
        //Udp
        IClientBuilder UseUdp(int port, int localPort);

        //Info
        IClientBuilder SetPacketBufferPoolSize(int size);
        IClientBuilder UseIp(string ip);
    }
}