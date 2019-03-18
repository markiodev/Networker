using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class ServerPacketProcessor : IServerPacketProcessor
    {
        private readonly ILogger logger;
        private readonly IPacketHandlers packetHandlers;
        private readonly IPacketSerialiser packetSerialiser;
        private readonly IServerInformation serverInformation;
        private readonly ObjectPool<ISender> tcpSenderObjectPool;
        private readonly ObjectPool<ISender> udpSenderObjectPool;
        private readonly ObjectPool<IPacketContext> packetContextObjectPool;
        private IUdpSocketSender _socketSender;

        public ServerPacketProcessor(ServerBuilderOptions options,
            ILogger<ServerPacketProcessor> logger,
            IPacketHandlers packetHandlers,
            IServerInformation serverInformation,
            IPacketSerialiser packetSerialiser)
        {
            this.logger = logger;
            this.packetHandlers = packetHandlers;
            this.serverInformation = serverInformation;
            this.packetSerialiser = packetSerialiser;

            tcpSenderObjectPool = new ObjectPool<ISender>(options.TcpMaxConnections);

            for (var i = 0; i < tcpSenderObjectPool.Capacity; i++)
                tcpSenderObjectPool.Push(new TcpSender(packetSerialiser));

            udpSenderObjectPool = new ObjectPool<ISender>(options.TcpMaxConnections * 2);

            for (var i = 0; i < udpSenderObjectPool.Capacity; i++)
                udpSenderObjectPool.Push(new UdpSender(_socketSender));

            packetContextObjectPool = new ObjectPool<IPacketContext>(options.TcpMaxConnections * 2);

            for (var i = 0; i < packetContextObjectPool.Capacity; i++)
                packetContextObjectPool.Push(new PacketContext());
        }

        public void SetUdpSender(IUdpSocketSender sender)
        {
            _socketSender = sender;
        }

        public void ProcessFromBuffer(ISender sender,
            byte[] buffer,
            int offset = 0,
            int length = 0,
            bool isTcp = true)
        {
            var bytesRead = 0;
            var currentPosition = offset;

            if (length == 0)
                length = buffer.Length;

            while (bytesRead < length)
            {
                var packetNameSize = packetSerialiser.CanReadName ? BitConverter.ToInt32(buffer, currentPosition) : 0;

                if (packetSerialiser.CanReadName) currentPosition += 4;

                var packetSize = packetSerialiser.CanReadLength ? BitConverter.ToInt32(buffer, currentPosition) : 0;

                if (packetSerialiser.CanReadLength) currentPosition += 4;

                try
                {
                    var packetTypeName = "Default";

                    if (packetSerialiser.CanReadName)
                        packetTypeName = Encoding.ASCII.GetString(buffer, currentPosition, packetNameSize);

                    var packetHandlerDictionary = packetHandlers.GetPacketHandlers();

                    if (!packetHandlerDictionary.ContainsKey(packetTypeName))
                    {
						logger.LogWarning($"Could not handle packet {packetTypeName}. Make sure you have registered a handler");
						return;
                    }

                    var packetHandler = packetHandlerDictionary[packetTypeName];

                    if (string.IsNullOrEmpty(packetTypeName))
                    {
                        if (isTcp)
                            serverInformation.InvalidTcpPackets++;
                        else
                            serverInformation.InvalidUdpPackets++;

                        logger.Error(new Exception("Packet was lost - Invalid"));
                        return;
                    }

                    if (packetSerialiser.CanReadName) currentPosition += packetNameSize;

                    var packetContext = this.packetContextObjectPool.Pop();
                    packetContext.Sender = sender;

                    if (packetSerialiser.CanReadOffset)
                    {
                        packetContext.PacketBytes = buffer;
                        packetHandler.Handle(buffer, currentPosition, packetSize, packetContext).GetAwaiter().GetResult();
                    }
                    else
                    {
                        var packetBytes = new byte[packetSize];
                        packetContext.PacketBytes = new byte[packetSize];
                        Buffer.BlockCopy(buffer, currentPosition, packetBytes, 0, packetSize);
                        Buffer.BlockCopy(buffer, currentPosition, packetContext.PacketBytes, 0, packetSize);
                        
                        packetHandler.Handle(packetBytes, packetContext).GetAwaiter().GetResult();
                    }

                    packetContextObjectPool.Push(packetContext);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }

                if (packetSerialiser.CanReadLength) currentPosition += packetSize;

                bytesRead += packetSize + packetNameSize;

                if (packetSerialiser.CanReadName) bytesRead += 4;

                if (packetSerialiser.CanReadLength) bytesRead += 4;

                if (isTcp)
                    serverInformation.ProcessedTcpPackets++;
                else
                    serverInformation.ProcessedUdpPackets++;
            }
        }

        public void ProcessTcp(SocketAsyncEventArgs socketEvent)
        {
            var sender = tcpSenderObjectPool.Pop() as TcpSender;
            try
            {
                sender.Socket = ((AsyncUserToken) socketEvent.UserToken).Socket;
                ProcessPacketsFromSocketEventArgs(sender, socketEvent);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            tcpSenderObjectPool.Push(sender);
        }

        public void ProcessUdp(SocketAsyncEventArgs socketEvent)
        {
            var sender = udpSenderObjectPool.Pop() as UdpSender;
            try
            {
                sender.RemoteEndpoint = socketEvent.RemoteEndPoint as IPEndPoint;
                ProcessPacketsFromSocketEventArgs(sender, socketEvent, false);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            udpSenderObjectPool.Push(sender);
        }

        public void ProcessUdpFromBuffer(EndPoint endPoint, byte[] buffer, int offset = 0, int length = 0)
        {
            var sender = udpSenderObjectPool.Pop() as UdpSender;
            try
            {
                sender.RemoteEndpoint = endPoint as IPEndPoint;

                ProcessFromBuffer(sender,
                    buffer,
                    offset,
                    length,
                    false);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            udpSenderObjectPool.Push(sender);
        }

        private void ProcessPacketsFromSocketEventArgs(ISender sender,
            SocketAsyncEventArgs eventArgs,
            bool isTcp = true)
        {
            ProcessFromBuffer(sender,
                eventArgs.Buffer,
                eventArgs.Offset,
                eventArgs.BytesTransferred,
                isTcp);
        }
    }
}