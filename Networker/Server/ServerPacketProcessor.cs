using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

        public ServerPacketProcessor(ServerBuilderOptions options,
            ILogger logger,
            IPacketHandlers packetHandlers,
            IServerInformation serverInformation,
            IPacketSerialiser packetSerialiser)
        {
            this.logger = logger;
            this.packetHandlers = packetHandlers;
            this.serverInformation = serverInformation;
            this.packetSerialiser = packetSerialiser;

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
                int packetNameSize = this.packetSerialiser.CanReadName ? BitConverter.ToInt32(buffer, currentPosition) : 0;

                if (this.packetSerialiser.CanReadName)
                {
                    currentPosition += 4;
                }

                int packetSize = this.packetSerialiser.CanReadLength ? BitConverter.ToInt32(buffer, currentPosition) : 0;

                if (this.packetSerialiser.CanReadLength)
                {
                    currentPosition += 4;
                }

                try
                {
                    string packetTypeName = "Default";

                    if (this.packetSerialiser.CanReadName)
                    {
                        packetTypeName = Encoding.ASCII.GetString(buffer, currentPosition, packetNameSize);
                        currentPosition += packetNameSize;
                    }

                    if(string.IsNullOrEmpty(packetTypeName))
                    {
                        if(isTcp)
                            this.serverInformation.InvalidTcpPackets++;
                        else
                            this.serverInformation.InvalidUdpPackets++;

                        this.logger.Error(new Exception("Packet was lost - Invalid"));
                        return;
                    }

                    var packetHandler = this.packetHandlers.GetPacketHandlers()[packetTypeName];

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
                }
                catch(Exception e)
                {
                    this.logger.Error(e);
                }

                if (this.packetSerialiser.CanReadLength)
                {
                    currentPosition += packetSize;
                }

                bytesRead += packetSize + packetNameSize;

                if (this.packetSerialiser.CanReadName)
                {
                    bytesRead += 4;
                }

                if (this.packetSerialiser.CanReadLength)
                {
                    bytesRead += 4;
                }

                if (isTcp)
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

        public void ProcessUdpFromBuffer(EndPoint endPoint, byte[] buffer, int offset = 0, int length = 0)
        {
            var sender = this.udpSenderObjectPool.Pop() as UdpSender;
            try
            {
                sender.RemoteEndpoint = endPoint;

                this.ProcessFromBuffer(sender,
                    buffer,
                    offset,
                    length,
                    false);
            }
            catch (Exception e)
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