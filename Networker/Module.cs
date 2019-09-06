using System.Collections.Generic;

namespace Networker
{
    public class Module : IModule
    {
        public Module()
        {
            PacketHandlers = new Dictionary<int, IPacketHandler>();
        }

        public Dictionary<int, IPacketHandler> PacketHandlers { get; }
    }
}