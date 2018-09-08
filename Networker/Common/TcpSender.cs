using System.Net;
using System.Net.Sockets;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public class TcpSender : ISender
    {
        private readonly IPacketSerialiser packetSerialiser;

        public TcpSender(IPacketSerialiser packetSerialiser)
        {
            this.packetSerialiser = packetSerialiser;
        }

        public Socket Socket { get; set; }

        public void Send<T>(T packet)
        {
            this.Socket.Send(this.packetSerialiser.Serialise(packet));
        }

        public EndPoint EndPoint => this.Socket.RemoteEndPoint;
    }
}