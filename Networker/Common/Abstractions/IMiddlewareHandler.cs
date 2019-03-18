using System.Threading.Tasks;

namespace Networker.Common.Abstractions
{
	public interface IMiddlewareHandler
	{
		Task<bool> Process(IPacketContext context);
	}
}