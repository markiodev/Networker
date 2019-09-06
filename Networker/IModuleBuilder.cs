using System;
using System.Collections.Generic;

namespace Networker
{
    public interface IModuleBuilder
    {
        IModule Build(IServiceProvider serviceProvider);
        void RegisterHandler<T>(int packetIdentifier) where T : IPacketHandler;
        void RegisterHandler(int packetIdentifier, Action<IPacketContext> packetContext);
        Dictionary<int, Type> GetRegisteredTypes();
    }
}