using System;
using System.Net.Sockets;
using SimpleNet.Common;
using SimpleNet.Interfaces;

namespace SimpleNet.Server
{
    public class TcpConnection : ISimpleNetConnection
    {
        public TcpConnection(Socket socket)
        {
            this.Socket = socket;
            this.Serializer = new PacketSerializer();
        }

        public PacketSerializer Serializer { get; }
        public Socket Socket { get; }
        
        public void Send<T>(T packet)
            where T: SimpleNetPacketBase
        {
            this.Socket.Send(this.Serializer.Serialize(packet));
        }
    }
}