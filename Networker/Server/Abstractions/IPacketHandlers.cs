using System;
using System.Collections.Generic;
using Networker.Common.Abstractions;

namespace Networker.Server.Abstractions
{
    public interface IPacketHandlers
    {
        Dictionary<string, IPacketHandler> GetPacketHandlers();
        void Add(string name, IPacketHandler packetHandler);
    }
}