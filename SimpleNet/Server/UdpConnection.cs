using System;
using System.Net.Sockets;
using SimpleNet.Common;
using SimpleNet.Interfaces;

namespace SimpleNet.Server
{
    public class UdpConnection : ISimpleNetConnection
    {
        private readonly UdpReceiveResult result;
        private readonly Socket socket;

        public UdpConnection(Socket socket, UdpReceiveResult result)
        {
            this.socket = socket;
            this.result = result;
        }

        public ISimpleNetServerPacketReceipt CreatePacket(SimpleNetPacketBase packet)
        {
            return null;
        }

        public void Send<T>(T packet)
            where T: SimpleNetPacketBase
        {
            var serializer = new PacketSerializer();
            this.socket.SendTo(serializer.Serialize(packet), this.result.RemoteEndPoint);
        }
    }
}