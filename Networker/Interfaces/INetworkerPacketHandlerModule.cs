using System;
using System.Collections.Generic;

namespace Networker.Interfaces
{
    public interface INetworkerPacketBaseHandlerModule
    {
        Dictionary<Type, Type> RegisterPacketHandlers();
    }
}