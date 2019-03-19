using System;
using System.Net.Sockets;

namespace Networker.Client.Abstractions
{
	public interface IClient
	{
		EventHandler<Socket> Connected { get; set; }
		EventHandler<Socket> Disconnected { get; set; }
		void Connect();
		long Ping();
		void Send<T>(T packet);
		void SendUdp<T>(T packet);
		void SendUdp(byte[] packet);
		void Stop();
	}
}