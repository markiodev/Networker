using System;
using System.Net.Sockets;
using Networker.Client.Abstractions;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server;
using Networker.Server.Abstractions;

namespace Networker.Client
{
    public class ClientPacketProcessor : IClientPacketProcessor
    {
        private readonly ObjectPool<byte[]> bytePool;
        private readonly ILogger logger;
        private readonly ClientBuilderOptions options;
        private readonly IPacketHandlers packetHandlers;
        private readonly IPacketSerialiser packetSerialiser;

        public ClientPacketProcessor(ClientBuilderOptions options,
            IPacketSerialiser packetSerialiser,
            ILogger logger,
            IPacketHandlers packetHandlers)
        {
            this.options = options;
            this.packetSerialiser = packetSerialiser;
            this.logger = logger;
            this.packetHandlers = packetHandlers;

            this.bytePool = new ObjectPool<byte[]>(50);

            for(var i = 0; i < this.bytePool.Capacity; i++)
            {
                this.bytePool.Push(new byte[options.PacketSizeBuffer]);
            }
        }

        public void Process(Socket socket)
        {
            var buffer = new byte[socket.Available];

            socket.Receive(buffer);

            this.Process(buffer,
                new TcpSender(this.packetSerialiser)
                {
                    Socket = socket
                });
        }

        public void Process(UdpReceiveResult data)
        {
            var buffer = data.Buffer;
            this.Process(buffer,
                new UdpSender(this.packetSerialiser)
                {
                    RemoteEndpoint = data.RemoteEndPoint
                });
        }

        private void Process(byte[] buffer, ISender sender)
        {
            int bytesRead = 0;
            int currentPosition = 0;

            while(bytesRead < buffer.Length)
            {
                int packetSize = BitConverter.ToInt32(buffer, currentPosition);

                if(packetSize == 0)
                {
                    break;
                }

                currentPosition += 4;

                var packetBytes = this.bytePool.Pop();

                if(buffer.Length - bytesRead < packetSize)
                {
                    this.logger.Error(new Exception("Packet was lost"));
                    return;
                }

                Buffer.BlockCopy(buffer, currentPosition, packetBytes, 0, packetSize);

                var deserialized = this.packetSerialiser.Deserialise<PacketBase>(packetBytes);

                if(string.IsNullOrEmpty(deserialized.UniqueKey))
                {
                    this.logger.Error(new Exception("Packet was lost - Invalid"));
                    return;
                }

                var packetHandler = this.packetHandlers.GetPacketHandlers()[deserialized.UniqueKey];

                packetHandler.Handle(packetBytes, sender);
                this.bytePool.Push(packetBytes);

                currentPosition += packetSize;
                bytesRead += packetSize + 4;
            }
        }
    }
}