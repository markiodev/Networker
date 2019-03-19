using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Client.Abstractions;
using Networker.Common;
using Networker.Common.Abstractions;

namespace Networker.Client
{
	public class Client : IClient
	{
		private readonly ILogger<Client> logger;
		private readonly ClientBuilderOptions options;
		private readonly IClientPacketProcessor packetProcessor;
		private readonly IPacketSerialiser packetSerialiser;
		private readonly PingOptions pingOptions;
		private readonly byte[] pingPacketBuffer;
		private bool isRunning = true;
		private Socket tcpSocket;
		private UdpClient udpClient;
		private IPEndPoint udpEndpoint;

		public Client(ClientBuilderOptions options,
			IPacketSerialiser packetSerialiser,
			IClientPacketProcessor packetProcessor,
			ILogger<Client> logger)
		{
			this.options = options;
			this.packetSerialiser = packetSerialiser;
			this.packetProcessor = packetProcessor;
			this.logger = logger;
			pingPacketBuffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaa");
			pingOptions = new PingOptions(64, true);
		}

		public EventHandler<Socket> Connected { get; set; }
		public EventHandler<Socket> Disconnected { get; set; }

		public void Connect()
		{
			if (options.TcpPort > 0 && tcpSocket == null)
			{
				tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				tcpSocket.Connect(options.Ip, options.TcpPort);
				Connected?.Invoke(this, tcpSocket);

				Task.Factory.StartNew(() =>
				{
					while (isRunning)
					{
						if (tcpSocket.Poll(10, SelectMode.SelectWrite)) packetProcessor.Process(tcpSocket);

						if (!tcpSocket.Connected)
						{
							Disconnected?.Invoke(this, tcpSocket);
							break;
						}
					}

					if (tcpSocket.Connected)
					{
						tcpSocket.Disconnect(false);
						tcpSocket.Close();
						Disconnected?.Invoke(this, tcpSocket);
					}

					tcpSocket = null;
				});
			}

			if (options.UdpPort > 0 && udpClient == null)
			{
				udpClient = new UdpClient();
				udpClient.ExclusiveAddressUse = false;
				udpClient.Client.SetSocketOption(SocketOptionLevel.Socket,
					SocketOptionName.ReuseAddress,
					true);

				var address = IPAddress.Parse(options.Ip);
				udpEndpoint = new IPEndPoint(address, options.UdpPort);
				udpClient.Client.Bind(udpEndpoint);

				Task.Factory.StartNew(() =>
				{
					logger.LogInformation(
						$"Connecting to UDP at {options.Ip}:{options.UdpPort}");

					while (isRunning)
						try
						{
							var data = udpClient.ReceiveAsync()
								.GetAwaiter()
								.GetResult();

							packetProcessor.Process(data);
						}
						catch (Exception ex)
						{
							logger.Error(ex);
						}

					udpClient = null;
				});
			}
		}

		public long Ping()
		{
			var pingSender = new Ping();
			var timeout = 10000;
			var reply = pingSender.Send(options.Ip, timeout, pingPacketBuffer, pingOptions);

			if (reply.Status == IPStatus.Success) return reply.RoundtripTime;

			logger.LogError("Could not get ping " + reply.Status);
			return -1;
		}

		public void Send<T>(T packet)
		{
			if (tcpSocket == null)
				throw new Exception("TCP client has not been initialised. Have you called .Connect()?");

			var serialisedPacket = packetSerialiser.Serialise(packet);

			var result = tcpSocket.Send(serialisedPacket);
		}

		public void SendUdp(byte[] packet)
		{
			if (udpClient == null)
				throw new Exception("UDP client has not been initialised. Have you called .Connect()?");

			udpClient.Send(packet, packet.Length, udpEndpoint);
		}

		public void SendUdp<T>(T packet)
		{
			if (udpClient == null)
				throw new Exception("UDP client has not been initialised. Have you called .Connect()?");

			var serialisedPacket = packetSerialiser.Serialise(packet);

			udpClient.Send(serialisedPacket, serialisedPacket.Length, udpEndpoint);
		}

		public void Stop()
		{
			isRunning = false;
		}
	}
}