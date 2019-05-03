using Microsoft.Extensions.Logging;
using Networker.Client.Abstractions;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;
using System;
using System.Net.Sockets;
using System.Text;

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

        private IUdpSocketSender _udpSocketSender;
        private ObjectPool<IPacketContext> _packetContextObjectPool;

        public ClientPacketProcessor(ClientBuilderOptions options,
            IPacketSerialiser packetSerialiser,
            ILogger<ClientPacketProcessor> logger,
            IPacketHandlers packetHandlers)
        {
            this.options = options;
            this.packetSerialiser = packetSerialiser;
            this.logger = logger;
            this.packetHandlers = packetHandlers;

            tcpSenderObjectPool = new ObjectPool<ISender>(options.ObjectPoolSize);

            for (var i = 0; i < tcpSenderObjectPool.Capacity; i++)
                tcpSenderObjectPool.Push(new TcpSender(packetSerialiser));

            udpSenderObjectPool = new ObjectPool<ISender>(options.ObjectPoolSize);

            for (var i = 0; i < udpSenderObjectPool.Capacity; i++)
                udpSenderObjectPool.Push(new UdpSender(_udpSocketSender));

            _packetContextObjectPool = new ObjectPool<IPacketContext>(options.ObjectPoolSize * 2);

            for (var i = 0; i < _packetContextObjectPool.Capacity; i++)
                _packetContextObjectPool.Push(new PacketContext()
                {
					Serialiser = this.packetSerialiser
                });

            bytePool = new ObjectPool<byte[]>(options.ObjectPoolSize);

            for (var i = 0; i < bytePool.Capacity; i++) bytePool.Push(new byte[options.PacketSizeBuffer]);
        }

        public void Process(Socket socket)
        {
            var buffer = bytePool.Pop();
            var sender = tcpSenderObjectPool.Pop();

            try
            {
                socket.Receive(buffer);

                var tcpSender = sender as TcpSender;
                tcpSender.Socket = socket;

                Process(buffer, sender);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                bytePool.Push(buffer);
                tcpSenderObjectPool.Push(sender);
            }
        }

        public void Process(UdpReceiveResult data)
        {
            var sender = udpSenderObjectPool.Pop();

            try
            {
                var buffer = data.Buffer;

                var udpSender = sender as UdpSender;
                udpSender.RemoteEndpoint = data.RemoteEndPoint;

                Process(buffer, sender);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                udpSenderObjectPool.Push(sender);
            }
        }

        public void SetUdpSocketSender(IUdpSocketSender socketSender)
        {
            _udpSocketSender = socketSender;
        }

        private void Process(byte[] buffer, ISender sender)
        {
            var bytesRead = 0;
            var currentPosition = 0;

            while (bytesRead < buffer.Length)
            {
                var packetTypeNameLength = packetSerialiser.CanReadName
                    ? BitConverter.ToInt32(buffer, currentPosition)
                    : 0;

                if (packetSerialiser.CanReadName) currentPosition += 4;

                var packetSize = packetSerialiser.CanReadLength
                    ? BitConverter.ToInt32(buffer, currentPosition)
                    : 0;

                if (packetSerialiser.CanReadLength) currentPosition += 4;

                var packetTypeName = "Default";

                if (packetSerialiser.CanReadName)
                {
                    if (buffer.Length - currentPosition < packetTypeNameLength)
                    {
                        return;
                    }

                    packetTypeName = Encoding.ASCII.GetString(buffer, currentPosition, packetTypeNameLength);
                    currentPosition += packetTypeNameLength;

                    if (string.IsNullOrEmpty(packetTypeName))
                    {
                        //logger.Error(new Exception("Packet was lost - Invalid"));
                        return;
                    }
                }

                var packetHandler = packetHandlers.GetPacketHandlers()[packetTypeName];

                if (packetSerialiser.CanReadLength && buffer.Length - bytesRead < packetSize)
                {
                    logger.Error(new Exception("Packet was lost"));
                    return;
                }

                var context = _packetContextObjectPool.Pop();
                context.Sender = sender;
				
                if (packetSerialiser.CanReadOffset)
                {
                    context.PacketBytes = buffer;
                    //packetHandler.Handle(buffer, currentPosition, packetSize, context).GetAwaiter().GetResult();
					//Not required/supported right now
                }
                else
                {
                    var packetBytes = new byte[packetSize];
                    Buffer.BlockCopy(buffer, currentPosition, packetBytes, 0, packetSize);
                    context.PacketBytes = packetBytes;
                    packetHandler.Handle(context).GetAwaiter().GetResult();
                }

                _packetContextObjectPool.Push(context);

                currentPosition += packetSize;
                bytesRead += packetSize + packetTypeNameLength;

                if (packetSerialiser.CanReadName) bytesRead += 4;
                if (packetSerialiser.CanReadLength) bytesRead += 4;
            }
        }
    }
}