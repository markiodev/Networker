using System;
using System.Collections.Generic;
using Networker.Common.Abstractions;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class PacketHandlers : IPacketHandlers
    {
        private readonly Dictionary<string, IPacketHandler> packetHandlers;

        public PacketHandlers()
        {
            this.packetHandlers = new Dictionary<string, IPacketHandler>();
        }

        public Dictionary<string, IPacketHandler> GetPacketHandlers()
        {
            return this.packetHandlers;
        }

        public void Add(string name, IPacketHandler packetHandler)
        {
            this.packetHandlers.Add(name, packetHandler);
        }
    }
}