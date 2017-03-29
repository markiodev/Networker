using System.Collections.Generic;

namespace SimpleNet
{
    public interface ISimpleNetPacketHandlerModule
    {
        Dictionary<object, object> RegisterPacketHandlers();
    }
}