using System.Net;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public class UdpSender : ISender
    {
        private readonly IPacketSerialiser packetSerialiser;

        public UdpSender(IPacketSerialiser packetSerialiser)
        {
            this.packetSerialiser = packetSerialiser;
        }

        public EndPoint RemoteEndpoint { get; set; }

        public void Send<T>(T packet)
        {
            //todo: Finish this
            this.packetSerialiser.Serialise(packet);
        }

        public EndPoint EndPoint => this.RemoteEndpoint;
    }
}