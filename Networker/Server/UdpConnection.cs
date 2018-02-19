using System;
using System.Net.Sockets;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Server
{
    public class UdpConnection : INetworkerConnection
    {
        private readonly UdpReceiveResult result;
        private readonly IPacketSerializer packetSerializer;
        private readonly Socket socket;

        public UdpConnection(Socket socket, UdpReceiveResult result, IPacketSerializer packetSerializer)
        {
            this.socket = socket;
            this.result = result;
            this.packetSerializer = packetSerializer;
        }

        public void Send<T>(T packet)
            where T: NetworkerPacketBase
        {
            this.socket.SendTo(this.packetSerializer.Serialize(packet), this.result.RemoteEndPoint);
        }
    }
}