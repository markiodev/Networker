using System;
using System.Net.Sockets;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class ServerPacketProcessor : IServerPacketProcessor
    {
        private readonly ObjectPool<byte[]> bytePool;
        private readonly ILogger logger;
        private readonly IPacketHandlers packetHandlers;
        private readonly IPacketIdentifierProvider packetIdentifierProvider;
        private readonly IServerInformation serverInformation;
        private readonly ObjectPool<ISender> tcpSenderObjectPool;
        private readonly ObjectPool<ISender> udpSenderObjectPool;

        public ServerPacketProcessor(ServerBuilderOptions options,
            ILogger logger,
            IPacketHandlers packetHandlers,
            IServerInformation serverInformation,
            IPacketIdentifierProvider packetIdentifierProvider,
            IPacketSerialiser packetSerialiser)
        {
            this.logger = logger;
            this.packetHandlers = packetHandlers;
            this.serverInformation = serverInformation;
            this.packetIdentifierProvider = packetIdentifierProvider;

            this.bytePool = new ObjectPool<byte[]>(options.TcpMaxConnections * 2);

            for(var i = 0; i < this.bytePool.Capacity; i++)
            {
                this.bytePool.Push(new byte[options.PacketSizeBuffer]);
            }

            this.tcpSenderObjectPool = new ObjectPool<ISender>(options.TcpMaxConnections);

            for(var i = 0; i < this.tcpSenderObjectPool.Capacity; i++)
            {
                this.tcpSenderObjectPool.Push(new TcpSender(packetSerialiser));
            }

            this.udpSenderObjectPool = new ObjectPool<ISender>(options.TcpMaxConnections * 2);

            for(var i = 0; i < this.udpSenderObjectPool.Capacity; i++)
            {
                this.udpSenderObjectPool.Push(new UdpSender(packetSerialiser));
            }
        }

        public void ProcessFromBuffer(ISender sender,
            byte[] buffer,
            int offset = 0,
            int length = 0,
            bool isTcp = true)
        {
            int bytesRead = 0;
            int currentPosition = offset;

            if(length == 0)
                length = buffer.Length;

            while(bytesRead < length)
            {
                int packetSize = BitConverter.ToInt32(buffer, currentPosition);

                if(packetSize == 0)
                {
                    break;
                }

                currentPosition += 4;

                var packetBytes = this.bytePool.Pop();
                try
                {
                    if(length - bytesRead < packetSize)
                    {
                        if(isTcp)
                            this.serverInformation.InvalidTcpPackets++;
                        else
                            this.serverInformation.InvalidUdpPackets++;

                        this.logger.Error(new Exception("Packet was lost"));
                        return;
                    }

                    Buffer.BlockCopy(buffer, currentPosition, packetBytes, 0, packetSize);

                    var packetIdentifier = this.packetIdentifierProvider.Provide(packetBytes);

                    if(string.IsNullOrEmpty(packetIdentifier))
                    {
                        if(isTcp)
                            this.serverInformation.InvalidTcpPackets++;
                        else
                            this.serverInformation.InvalidUdpPackets++;

                        this.logger.Error(new Exception("Packet was lost - Invalid"));
                        return;
                    }

                    var packetHandler = this.packetHandlers.GetPacketHandlers()[packetIdentifier];

                    packetHandler.Handle(packetBytes, sender);
                }
                catch(Exception e)
                {
                    this.logger.Error(e);
                }
                finally
                {
                    this.bytePool.Push(packetBytes);
                }

                currentPosition += packetSize;
                bytesRead += packetSize + 4;
                if(isTcp)
                    this.serverInformation.ProcessedTcpPackets++;
                else
                    this.serverInformation.ProcessedUdpPackets++;
            }
        }

        public void ProcessTcp(SocketAsyncEventArgs socketEvent)
        {
            var sender = this.tcpSenderObjectPool.Pop() as TcpSender;
            try
            {
                sender.Socket = ((AsyncUserToken)socketEvent.UserToken).Socket;
                this.ProcessPacketsFromSocketEventArgs(sender, socketEvent);
            }
            catch(Exception e)
            {
                this.logger.Error(e);
            }

            this.tcpSenderObjectPool.Push(sender);
        }

        public void ProcessUdp(SocketAsyncEventArgs socketEvent)
        {
            var sender = this.udpSenderObjectPool.Pop() as UdpSender;
            try
            {
                sender.RemoteEndpoint = socketEvent.RemoteEndPoint;
                this.ProcessPacketsFromSocketEventArgs(sender, socketEvent, false);
            }
            catch(Exception e)
            {
                this.logger.Error(e);
            }

            this.udpSenderObjectPool.Push(sender);
        }

        private void ProcessPacketsFromSocketEventArgs(ISender sender,
            SocketAsyncEventArgs eventArgs,
            bool isTcp = true)
        {
            this.ProcessFromBuffer(sender,
                eventArgs.Buffer,
                eventArgs.Offset,
                eventArgs.BytesTransferred,
                isTcp);
        }
    }
}