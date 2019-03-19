using Networker.Common.Abstractions;

namespace Networker.Server.Abstractions
{
	public interface IServerBuilder : IBuilder<IServerBuilder, IServer>
	{
		//Info
		IServerBuilder SetMaximumConnections(int maxConnections);

		//Tcp
		IServerBuilder UseTcpSocketListener<T>()
			where T : class, ITcpSocketListenerFactory;

		//Udp
		IServerBuilder UseUdpSocketListener<T>()
			where T : class, IUdpSocketListenerFactory;
	}
}