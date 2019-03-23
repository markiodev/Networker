using System;
using System.Threading.Tasks;

namespace Networker.Common.Abstractions
{
	public interface IMiddlewareHandler
	{
		Task Process(IPacketContext context, Action nextMiddleware);
	}
}