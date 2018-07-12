using System;
using System.Net;
using System.Net.Sockets;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class UdpSocketListener : IUdpSocketListener
    {
        private readonly IBufferManager bufferManager;
        private readonly ILogger logger;
        private readonly ServerBuilderOptions options;
        private readonly IServerPacketProcessor serverPacketProcessor;
        private readonly ObjectPool<SocketAsyncEventArgs> socketEventArgsPool;
        private IPEndPoint endPoint;
        private Socket listener;

        public UdpSocketListener(ServerBuilderOptions options,
            ILogger logger,
            IServerPacketProcessor serverPacketProcessor,
            IBufferManager bufferManager)
        {
            this.options = options;
            this.logger = logger;
            this.serverPacketProcessor = serverPacketProcessor;
            this.bufferManager = bufferManager;
            this.socketEventArgsPool = new ObjectPool<SocketAsyncEventArgs>(options.UdpSocketObjectPoolSize);
        }

        public IPEndPoint GetEndPoint()
        {
            return this.endPoint;
        }

        public Socket GetSocket()
        {
            return this.listener;
        }

        public void Listen()
        {
            this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            this.endPoint = new IPEndPoint(IPAddress.Loopback, this.options.UdpPort);
            this.listener.Bind(this.endPoint);

            for(var i = 0; i < this.options.UdpSocketObjectPoolSize; i++)
            {
                var socketEventArgs = new SocketAsyncEventArgs();
                socketEventArgs.Completed += this.ProcessReceivedData;
                socketEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, this.options.UdpPort);

                this.socketEventArgsPool.Push(socketEventArgs);
            }

            this.logger.Info($"Starting UDP listener on port {this.options.UdpPort}.");

            this.Process();
        }

        private void Process()
        {
            var recieveArgs = this.socketEventArgsPool.Pop();
            this.bufferManager.SetBuffer(recieveArgs);

            if(!this.listener.ReceiveFromAsync(recieveArgs))
            {
                this.ProcessReceivedData(this, recieveArgs);
            }
        }

        private void ProcessReceivedData(object sender, SocketAsyncEventArgs e)
        {
            this.Process();

            if(e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                this.serverPacketProcessor.ProcessUdp(e);
            }

            this.socketEventArgsPool.Push(e);
        }
    }
}