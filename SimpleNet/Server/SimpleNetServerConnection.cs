using System;
using System.Net.Sockets;
using SimpleNet.Common;
using SimpleNet.Interfaces;

namespace SimpleNet.Server
{
    public class SimpleNetServerConnection : ISimpleNetConnection
    {
        public SimpleNetServerConnection(Socket socket)
        {
            this.Socket = socket;
            this.Serializer = new PacketSerializer();
        }

        public PacketSerializer Serializer { get; }
        public Socket Socket { get; }

        public ISimpleNetServerPacketReceipt CreatePacket(SimpleNetPacketBase packet)
        {
            throw new NotImplementedException();
        }

        public void Send<T>(T packet)
            where T: SimpleNetPacketBase
        {
            this.Socket.Send(this.Serializer.Serialize(packet));
        }
    }
}