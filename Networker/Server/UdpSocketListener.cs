using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Networker.Common;
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
			ILogger<UdpSocketListener> logger,
			IServerPacketProcessor serverPacketProcessor,
			IBufferManager bufferManager)
		{
			this.options = options;
			this.logger = logger;
			this.serverPacketProcessor = serverPacketProcessor;
			this.bufferManager = bufferManager;
			socketEventArgsPool = new ObjectPool<SocketAsyncEventArgs>(options.UdpSocketObjectPoolSize);
		}

		public IPEndPoint GetEndPoint()
		{
			return endPoint;
		}

		public Socket GetSocket()
		{
			return listener;
		}

		public void Listen()
		{
			listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			endPoint = new IPEndPoint(IPAddress.Loopback, options.UdpPort);
			listener.Bind(endPoint);

			for (var i = 0; i < options.UdpSocketObjectPoolSize; i++)
			{
				var socketEventArgs = new SocketAsyncEventArgs();
				socketEventArgs.Completed += ProcessReceivedData;
				socketEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, options.UdpPort);

				socketEventArgsPool.Push(socketEventArgs);
			}

			logger.LogDebug($"Starting UDP listener on port {options.UdpPort}.");
			listener.Listen(10000);
			Process();
		}

		private void Process()
		{
			var recieveArgs = socketEventArgsPool.Pop();
			bufferManager.SetBuffer(recieveArgs);

			if (!listener.ReceiveFromAsync(recieveArgs)) ProcessReceivedData(this, recieveArgs);

			StartAccept(recieveArgs);
		}

		private void ProcessReceivedData(object sender, SocketAsyncEventArgs e)
		{
			Process();

			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				e.SetBuffer(e.Offset, e.BytesTransferred);
				serverPacketProcessor.ProcessUdp(e);
			}

			socketEventArgsPool.Push(e);
		}

		private void StartAccept(SocketAsyncEventArgs acceptEventArg)
		{
			var willRaiseEvent = listener.AcceptAsync(acceptEventArg);

			if (!willRaiseEvent) ProcessReceivedData(this, acceptEventArg);
		}
	}
}