using System;
using System.Collections.Generic;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetPacketBaseHandlerModule
    {
        Dictionary<Type, Type> RegisterPacketHandlers();
    }
}