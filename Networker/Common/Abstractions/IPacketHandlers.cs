using System.Collections.Generic;

namespace Networker.Common.Abstractions
{
    public interface IPacketHandlers
    {
        Dictionary<string, IPacketHandler> GetPacketHandlers();
        void Add(string name, IPacketHandler packetHandler);
    }
}