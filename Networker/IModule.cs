using System.Collections.Generic;

namespace Networker
{
    public interface IModule
    {
        Dictionary<int, IPacketHandler> PacketHandlers { get; }
    }
}