using System;
using System.Collections.Generic;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public abstract class PacketHandlerModuleBase : IPacketHandlerModule
    {
        private readonly Dictionary<Type, Type> _packetHandlers = new Dictionary<Type, Type>();

        public void AddPacketHandler<TPacket, TPacketHandler>()
        {
            this._packetHandlers.Add(typeof(TPacket), typeof(TPacketHandler));
        }

        public Dictionary<Type, Type> GetPacketHandlers()
        {
            return this._packetHandlers;
        }
    }
}