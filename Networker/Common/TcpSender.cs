using System.Net;
using System.Net.Sockets;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public class TcpSender : ISender
    {
        private readonly IPacketSerialiser _packetSerialiser;

        public TcpSender(IPacketSerialiser packetSerialiser)
        {
            this._packetSerialiser = packetSerialiser;
        }

        public Socket Socket { get; set; }

        public void Send<T>(T packet)
        {
            this.Socket.Send(this._packetSerialiser.Serialise(packet));
        }

        public IPEndPoint EndPoint => this.Socket.RemoteEndPoint as IPEndPoint;
    }
}