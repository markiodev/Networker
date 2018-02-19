using System;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using Networker.Common;
using Networker.Common.Encryption;
using Networker.Interfaces;

namespace Networker.Server
{
    public class TcpConnection : INetworkerConnection
    {
        private readonly IPacketEncryption packetEncryption;
        private readonly IPacketSerializer packetSerializer;

        public TcpConnection(Socket socket,
            IPacketSerializer packetSerializer,
            IPacketEncryption packetEncryption)
        {
            this.packetSerializer = packetSerializer;
            this.packetEncryption = packetEncryption;
            this.Socket = socket;
            this.Identifier = Guid.NewGuid()
                                  .ToString();
        }

        public string Identifier { get; }
        public Socket Socket { get; }

        public void Close()
        {
            this.Socket.Shutdown(SocketShutdown.Both);
        }

        public void Send<T>(T packet)
            where T: NetworkerPacketBase
        {
            var serializedPacket = this.packetSerializer.Serialize(packet);

            if(this.packetEncryption == null)
            {
                this.Socket.Send(serializedPacket);
            }
            else
            {
                var encryptedData = this.packetEncryption.GetEncryptor()
                                        .Encrypt(serializedPacket);

                this.Socket.Send(this.packetSerializer.Serialize(new EncryptedPacket
                                                                 {
                                                                     Data = encryptedData
                                                                 }));
            }
        }
    }
}