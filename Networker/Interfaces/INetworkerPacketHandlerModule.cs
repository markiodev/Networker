using System;
using System.Collections.Generic;

namespace Networker.Interfaces
{
    public interface INetworkerPacketHandlerModule
    {
        Dictionary<Type, Type> RegisterPacketHandlers();
    }
}