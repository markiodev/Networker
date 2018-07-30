using System;
using System.Net;
using System.Net.Sockets;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Server
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