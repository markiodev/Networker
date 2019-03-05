using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Server.Abstractions;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Networker.Server
{
    public class TcpSocketListener : ITcpSocketListener
    {
        private readonly IBufferManager bufferManager;
        private readonly ILogger logger;
        private readonly ITcpConnections tcpConnections;
        readonly Semaphore maxNumberAcceptedClients;
        private readonly ServerBuilderOptions serverBuilderOptions;
        private readonly IServerPacketProcessor serverPacketProcessor;
        private readonly ObjectPool<SocketAsyncEventArgs> socketEventArgsPool;
        private Socket listenSocket;

        public TcpSocketListener(ServerBuilderOptions serverBuilderOptions,
            IServerPacketProcessor serverPacketProcessor,
            IBufferManager bufferManager,
            ILogger<TcpSocketListener> logger,
            ITcpConnections tcpConnections)
        {
            this.serverBuilderOptions = serverBuilderOptions;
            this.serverPacketProcessor = serverPacketProcessor;
            this.bufferManager = bufferManager;
            this.logger = logger;
            this.tcpConnections = tcpConnections;
            this.socketEventArgsPool =
                new ObjectPool<SocketAsyncEventArgs>(serverBuilderOptions.TcpMaxConnections);
            this.maxNumberAcceptedClients = new Semaphore(this.serverBuilderOptions.TcpMaxConnections,
                this.serverBuilderOptions.TcpMaxConnections);
        }

        public EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        public EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }

        public void EventArgCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }

        public Socket GetSocket()
        {
            return this.listenSocket;
        }

        public void Listen()
        {
            this.listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.Bind(new IPEndPoint(IPAddress.Any, this.serverBuilderOptions.TcpPort));

            for (int i = 0; i < this.serverBuilderOptions.TcpMaxConnections; i++)
            {
                var socketAsyncEventArgs = new SocketAsyncEventArgs();
                socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(this.Completed);
                socketAsyncEventArgs.UserToken = new AsyncUserToken();

                this.bufferManager.SetBuffer(socketAsyncEventArgs);

                this.socketEventArgsPool.Push(socketAsyncEventArgs);
            }

            this.listenSocket.Listen(this.serverBuilderOptions.TcpMaxConnections);
            this.StartAccept(null);

            this.logger.LogInformation($"Started TCP listener on port {this.serverBuilderOptions.TcpPort}.");
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            this.logger.LogInformation(
                $"TCP Client Disconnected. IP: {(token.Socket.RemoteEndPoint as IPEndPoint).Address}");

            var connection =
                this.tcpConnections.GetConnections().FirstOrDefault(f => f.Socket == ((AsyncUserToken)e.UserToken).Socket);

            if (connection != null)
            {
                this.tcpConnections.Remove(connection);
            }

            this.ClientDisconnected?.Invoke(this,
                new TcpConnectionDisconnectedEventArgs(new TcpConnection(token.Socket)));

            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception) { }

            this.socketEventArgsPool.Push(e);
        }

        private void Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException(
                        "The last operation completed on the socket was not a receive or send");
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs readEventArgs = this.socketEventArgsPool.Pop();

            ((AsyncUserToken)readEventArgs.UserToken).Socket = e.AcceptSocket;

            var connection = this.tcpConnections.Add(((AsyncUserToken)readEventArgs.UserToken).Socket);

            this.logger.LogDebug(
                $"TCP Client Connected. IP: {(connection.Socket.RemoteEndPoint as IPEndPoint).Address}");

            this.ClientConnected?.Invoke(this, new TcpConnectionConnectedEventArgs(connection));

            this.maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
            {
                this.ProcessReceive(readEventArgs);
            }

            this.StartAccept(e);
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    this.serverPacketProcessor.ProcessTcp(e);

                    this.ProcessSend(e);
                }
                else
                {
                    this.CloseClientSocket(e);
                }
            }
            catch (Exception exception)
            {
                this.logger.Error(exception);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                AsyncUserToken token = (AsyncUserToken)e.UserToken;

                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    this.ProcessReceive(e);
                }
            }
            else
            {
                this.CloseClientSocket(e);
            }
        }

        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            try
            {
                if (acceptEventArg == null)
                {
                    acceptEventArg = new SocketAsyncEventArgs();
                    acceptEventArg.Completed +=
                        new EventHandler<SocketAsyncEventArgs>(this.EventArgCompleted);
                }
                else
                {
                    acceptEventArg.AcceptSocket = null;
                }

                bool willRaiseEvent = this.listenSocket.AcceptAsync(acceptEventArg);
                if (!willRaiseEvent)
                {
                    this.ProcessAccept(acceptEventArg);
                }
            }
            catch (Exception e)
            {
                this.logger.Error(e);
            }
        }
    }
}
