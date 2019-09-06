using System;
using System.Collections.Generic;
using Networker.PacketHandlers;

namespace Networker
{
    public class ModuleBuilder : IModuleBuilder
    {
        private IModule _module;
        private readonly Dictionary<int, Type> _handlers;

        public ModuleBuilder()
        {
            _handlers = new Dictionary<int, Type>();
            _module = new Module();
        }

        public Dictionary<int, Type> GetRegisteredTypes()
        {
            return _handlers;
        }

        public IModule Build(IServiceProvider serviceProvider)
        {
            foreach (var handler in _handlers)
            {
                _module.PacketHandlers.Add(handler.Key, serviceProvider.GetService(handler.Value) as IPacketHandler);
            }

            return _module;
        }

        public void RegisterHandler<T>(int packetIdentifier) where T : IPacketHandler
        {
            _handlers.Add(packetIdentifier, typeof(T));
        }

        public void RegisterHandler(int packetIdentifier, Action<IPacketContext> action)
        {
            _module.PacketHandlers.Add(packetIdentifier, new FunctionHandler(action));
        }
    }
}