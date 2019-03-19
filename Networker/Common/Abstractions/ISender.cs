using System.Net;

namespace Networker.Common.Abstractions
{
	public interface ISender
	{
		IPEndPoint EndPoint { get; }
		void Send<T>(T packet);
	}
}