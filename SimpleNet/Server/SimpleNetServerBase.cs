using System;

namespace SimpleNet
{
    public abstract class SimpleNetServerBase : ISimpleNetServer
    {
        public ISimpleNetServer Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public ISimpleNetServer RegisterPacketHandler<TPacketType, TPacketHandlerType>()
            where TPacketHandlerType : ISimpleNetServerPacketHandler<TPacketType>
        {
            throw new NotImplementedException();
        }

        public ISimpleNetServer RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            throw new NotImplementedException();
        }
    }
}