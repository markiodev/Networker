using Networker.Common.Abstractions;

namespace Networker.Client.Abstractions
{
	public interface IClientBuilder : IBuilder<IClientBuilder, IClient>
	{
		T Build<T>()
			where T : IClient;

		//Info
		IClientBuilder SetPacketBufferPoolSize(int size);

		IClientBuilder UseIp(string ip);

		//Udp
		IClientBuilder UseUdp(int port, int localPort);
	}
}