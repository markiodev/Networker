using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Networker.Common.Abstractions;

namespace Networker.Example.Json.Middleware
{
	public class RoleCheckMiddleware : IMiddlewareHandler
	{
		private readonly ILogger<RoleCheckMiddleware> logger;

		public RoleCheckMiddleware(ILogger<RoleCheckMiddleware> logger)
		{
			this.logger = logger;
		}

		public async Task Process(IPacketContext context, Action nextMiddleware)
		{
			var roleAttribute = context.Handler.GetType()
				.GetCustomAttribute<RoleRequired>();

			if (roleAttribute == null)
			{
				nextMiddleware.Invoke();
				return;
			}

			if (roleAttribute.RoleName == "Admin" && IsAdmin(context.Sender))
			{
				nextMiddleware.Invoke();
				return;
			}

			logger.LogCritical("Somebody tried to do something they did not have permission for!");
			context.Sender.Send(new NotAllowedResponsePacket());
		}

		private bool IsAdmin(ISender contextSender)
		{
			//Add some BETTER custom logic here
			return contextSender.EndPoint.Address == IPAddress.Parse("127.0.0.1");
		}
	}
}