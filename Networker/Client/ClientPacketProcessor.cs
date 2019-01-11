using System;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Networker.Client.Abstractions;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Client
{
    public class ClientPacketProcessor : IClientPacketProcessor
    {
        private readonly ObjectPool<byte[]> bytePool;
        private readonly ILogger logger;
        private readonly ClientBuilderOptions options;
        private readonly IPacketHandlers packetHandlers;
        private readonly IPacketSerialiser packetSerialiser;
        private readonly ObjectPool<ISender> tcpSenderObjectPool;
        private readonly ObjectPool<ISender> udpSenderObjectPool;

        public ClientPacketProcessor(ClientBuilderOptions options,
            IPacketSerialiser packetSerialiser,
            ILogger<ClientPacketProcessor> logger,
            IPacketHandlers packetHandlers)
        {
            this.options = options;
            this.packetSerialiser = packetSerialiser;
            this.logger = logger;
            this.packetHandlers = packetHandlers;

            this.tcpSenderObjectPool = new ObjectPool<ISender>(options.ObjectPoolSize);

            for(var i = 0; i < this.tcpSenderObjectPool.Capacity; i++)
            {
                this.tcpSenderObjectPool.Push(new TcpSender(packetSerialiser));
            }

            this.udpSenderObjectPool = new ObjectPool<ISender>(options.ObjectPoolSize);

            for(var i = 0; i < this.udpSenderObjectPool.Capacity; i++)
            {
                this.udpSenderObjectPool.Push(new UdpSender(packetSerialiser));
            }

            this.bytePool = new ObjectPool<byte[]>(options.ObjectPoolSize);

            for(int i = 0; i < this.bytePool.Capacity; i++)
            {
                this.bytePool.Push(new byte[options.PacketSizeBuffer]);
            }
        }

        public void Process(Socket socket)
        {
            var buffer = this.bytePool.Pop();
            var sender = this.tcpSenderObjectPool.Pop();

            try
            {
                socket.Receive(buffer);

                var tcpSender = sender as TcpSender;
                tcpSender.Socket = socket;

                this.Process(buffer, sender);
            }
            catch(Exception ex)
            {
                this.logger.Error(ex);
            }
            finally
            {
                this.bytePool.Push(buffer);
                this.tcpSenderObjectPool.Push(sender);
            }
        }

        public void Process(UdpReceiveResult data)
        {
            var sender = this.udpSenderObjectPool.Pop();

            try
            {
                var buffer = data.Buffer;

                var udpSender = sender as UdpSender;
                udpSender.RemoteEndpoint = data.RemoteEndPoint;

                this.Process(buffer, sender);
            }
            catch(Exception ex)
            {
                this.logger.Error(ex);
            }
            finally
            {
                this.udpSenderObjectPool.Push(sender);
            }
        }

        private void Process(byte[] buffer, ISender sender)
        {
            int bytesRead = 0;
            int currentPosition = 0;

            while(bytesRead < buffer.Length)
            {
                int packetTypeNameLength = this.packetSerialiser.CanReadName
                                               ? BitConverter.ToInt32(buffer, currentPosition)
                                               : 0;

                if(this.packetSerialiser.CanReadName)
                {
                    currentPosition += 4;
                }

                int packetSize = this.packetSerialiser.CanReadLength
                                     ? BitConverter.ToInt32(buffer, currentPosition)
                                     : 0;

                if(this.packetSerialiser.CanReadLength)
                {
                    currentPosition += 4;
                }

                string packetTypeName = "Default";

                if(!this.packetSerialiser.CanReadName)
                {
                    packetTypeName = Encoding.ASCII.GetString(buffer, currentPosition, packetTypeNameLength);
                    currentPosition += packetTypeNameLength;

                    if(string.IsNullOrEmpty(packetTypeName))
                    {
                        this.logger.Error(new Exception("Packet was lost - Invalid"));
                        return;
                    }
                }

                var packetHandler = this.packetHandlers.GetPacketHandlers()[packetTypeName];

                if(this.packetSerialiser.CanReadLength)
                {
                    if(buffer.Length - bytesRead < packetSize)
                    {
                        this.logger.Error(new Exception("Packet was lost"));
                        return;
                    }
                }

                if(this.packetSerialiser.CanReadOffset)
                {
                    packetHandler.Handle(buffer, currentPosition, packetSize, sender);
                }
                else
                {
                    var packetBytes = new byte[packetSize];
                    Buffer.BlockCopy(buffer, currentPosition, packetBytes, 0, packetSize);
                    packetHandler.Handle(packetBytes, sender);
                }

                currentPosition += packetSize;
                bytesRead += packetSize + packetTypeNameLength;

                if(this.packetSerialiser.CanReadName)
                {
                    bytesRead += 4;
                }

                if(this.packetSerialiser.CanReadLength)
                {
                    bytesRead += 4;
                }
            }
        }
    }
}