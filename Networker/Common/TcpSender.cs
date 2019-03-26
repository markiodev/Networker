using System.Net;
using System.Net.Sockets;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public class TcpSender : ISender
    {
        public IPEndPoint EndPoint => this.Socket.RemoteEndPoint as IPEndPoint;

        public Socket Socket { get; set; }

        private readonly IPacketSerialiser _packetSerialiser;

        public TcpSender(IPacketSerialiser packetSerialiser)
        {
            this._packetSerialiser = packetSerialiser;
        }

        /// <inheritdoc />
        public void Send<T>(T packet) => Send(this._packetSerialiser.Serialise(packet));

        private void Send(byte[] packet)
        {
            this.Socket.Send(packet);
        }
    }
}
