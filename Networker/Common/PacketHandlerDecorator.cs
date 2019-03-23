using Networker.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Networker.Common
{
    public abstract class PacketHandlerDecorator<T> : PacketHandlerBase<T> where T: class
    {
		private PacketHandlerBase<T> decoratedHandler;
		private Dictionary<Type, MethodInfo> processMethodInfoCache = new Dictionary<Type, MethodInfo>();

		public void Decorate(PacketHandlerBase<T> handler) 
		{
			decoratedHandler = handler;
		}

		protected Task ContinueProcessing(T packet, IPacketContext context) 
		{
			return decoratedHandler.Process(packet, context);
		}

		protected MethodInfo GetProcessMethodInfo(T packet) 
		{
			return GetProcessMethodInfo(packet.GetType());
		}

		protected MethodInfo GetProcessMethodInfo<S>() where S : T 
		{
			return GetProcessMethodInfo(typeof(S));
		}

		protected MethodInfo GetProcessMethodInfo(Type packetType) 
		{
			if (processMethodInfoCache.ContainsKey(packetType)) 
			{
				return processMethodInfoCache[packetType];
			}

			MethodInfo processMethodInfo = null;
			if (decoratedHandler is PacketHandlerDecorator<T> decoratorHandler)
			{
				processMethodInfo = decoratorHandler.GetProcessMethodInfo(packetType);
			} 
			else
			{
				Type[] processParameterTypes = new Type[] { packetType, typeof(IPacketContext) };
				processMethodInfo = GetType().GetMethod("Process", processParameterTypes);
			}
			processMethodInfoCache.Add(packetType, processMethodInfo);
			return processMethodInfo;
		}
    }
}