using System;
using System.Net.Sockets;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.Server
{
    public class UdpConnection : INetworkerConnection
    {
        private readonly UdpReceiveResult result;
        private readonly Socket socket;

        public UdpConnection(Socket socket, UdpReceiveResult result)
        {
            this.socket = socket;
            this.result = result;
        }

        public void Send<T>(T packet)
            where T: NetworkerPacketBase
        {
            var serializer = new PacketSerializer();
            this.socket.SendTo(serializer.Serialize(packet), this.result.RemoteEndPoint);
        }
    }
}