using System;
using System.Collections.Generic;
using Networker.Interfaces;

namespace Networker.Common
{
    public class DefaultPacketHandlerModule : INetworkerPacketHandlerModule
    {
        public DefaultPacketHandlerModule()
        {
            this.Modules = new Dictionary<Type, Type>();
        }

        public Dictionary<Type, Type> Modules { get; set; }

        public Dictionary<Type, Type> RegisterPacketHandlers()
        {
            return this.Modules;
        }
    }
}