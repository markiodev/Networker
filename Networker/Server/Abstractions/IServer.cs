using System;

namespace Networker.Server.Abstractions
{
	public interface IServer
	{
		IServerInformation Information { get; }
		EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
		EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }
		EventHandler<ServerInformationEventArgs> ServerInformationUpdated { get; set; }
		void Broadcast<T>(T packet);
		ITcpConnections GetConnections();
		void Start();
		void Stop();
	}
}