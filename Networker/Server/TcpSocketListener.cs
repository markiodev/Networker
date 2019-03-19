using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class TcpSocketListener : ITcpSocketListener
	{
		private readonly IBufferManager bufferManager;
		private readonly ILogger logger;
		private readonly Semaphore maxNumberAcceptedClients;
		private readonly ServerBuilderOptions serverBuilderOptions;
		private readonly IServerPacketProcessor serverPacketProcessor;
		private readonly ObjectPool<SocketAsyncEventArgs> socketEventArgsPool;
		private readonly ITcpConnections tcpConnections;
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
			socketEventArgsPool =
				new ObjectPool<SocketAsyncEventArgs>(serverBuilderOptions.TcpMaxConnections);
			maxNumberAcceptedClients = new Semaphore(this.serverBuilderOptions.TcpMaxConnections,
				this.serverBuilderOptions.TcpMaxConnections);
		}

		public EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
		public EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }

		public Socket GetSocket()
		{
			return listenSocket;
		}

		public void Listen()
		{
			listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listenSocket.Bind(new IPEndPoint(IPAddress.Any, serverBuilderOptions.TcpPort));

			for (var i = 0; i < serverBuilderOptions.TcpMaxConnections; i++)
			{
				var socketAsyncEventArgs = new SocketAsyncEventArgs();
				socketAsyncEventArgs.Completed += Completed;
				socketAsyncEventArgs.UserToken = new AsyncUserToken();

				bufferManager.SetBuffer(socketAsyncEventArgs);

				socketEventArgsPool.Push(socketAsyncEventArgs);
			}

			listenSocket.Listen(serverBuilderOptions.TcpMaxConnections);
			StartAccept(null);

			logger.LogInformation($"Started TCP listener on port {serverBuilderOptions.TcpPort}.");
		}

		public void EventArgCompleted(object sender, SocketAsyncEventArgs e)
		{
			ProcessAccept(e);
		}

		private void CloseClientSocket(SocketAsyncEventArgs e)
		{
			var token = e.UserToken as AsyncUserToken;

			logger.LogInformation(
				$"TCP Client Disconnected. IP: {(token.Socket.RemoteEndPoint as IPEndPoint).Address}");

			var connection = tcpConnections.GetConnections()
				.FirstOrDefault(f => f.Socket == ((AsyncUserToken) e.UserToken).Socket);

			if (connection != null) tcpConnections.Remove(connection);

			ClientDisconnected?.Invoke(this,
				new TcpConnectionDisconnectedEventArgs(new TcpConnection(token.Socket)));

			try
			{
				token.Socket.Shutdown(SocketShutdown.Send);
			}
			catch (Exception)
			{
			}

			socketEventArgsPool.Push(e);
		}

		private void Completed(object sender, SocketAsyncEventArgs e)
		{
			switch (e.LastOperation)
			{
				case SocketAsyncOperation.Receive:
					ProcessReceive(e);
					break;
				case SocketAsyncOperation.Send:
					ProcessSend(e);
					break;
				default:
					throw new ArgumentException(
						"The last operation completed on the socket was not a receive or send");
			}
		}

		private void ProcessAccept(SocketAsyncEventArgs e)
		{
			var readEventArgs = socketEventArgsPool.Pop();

			((AsyncUserToken) readEventArgs.UserToken).Socket = e.AcceptSocket;

			var connection = tcpConnections.Add(((AsyncUserToken) readEventArgs.UserToken).Socket);

			logger.LogDebug(
				$"TCP Client Connected. IP: {(connection.Socket.RemoteEndPoint as IPEndPoint).Address}");

			ClientConnected?.Invoke(this, new TcpConnectionConnectedEventArgs(connection));

			maxNumberAcceptedClients.WaitOne();
			var willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
			if (!willRaiseEvent) ProcessReceive(readEventArgs);

			StartAccept(e);
		}

		private void ProcessReceive(SocketAsyncEventArgs e)
		{
			try
			{
				if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
				{
					serverPacketProcessor.ProcessTcp(e);

					ProcessSend(e);
				}
				else
				{
					CloseClientSocket(e);
				}
			}
			catch (Exception exception)
			{
				logger.Error(exception);
			}
		}

		private void ProcessSend(SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				var token = (AsyncUserToken) e.UserToken;

				var willRaiseEvent = token.Socket.ReceiveAsync(e);
				if (!willRaiseEvent) ProcessReceive(e);
			}
			else
			{
				CloseClientSocket(e);
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
						EventArgCompleted;
				}
				else
				{
					acceptEventArg.AcceptSocket = null;
				}

				var willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
				if (!willRaiseEvent) ProcessAccept(acceptEventArg);
			}
			catch (Exception e)
			{
				logger.Error(e);
			}
		}
	}
}