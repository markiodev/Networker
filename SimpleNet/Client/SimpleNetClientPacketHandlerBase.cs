using System;
using SimpleNet.Common;
using SimpleNet.Interfaces;
using ZeroFormatter;

namespace SimpleNet.Client
{
    public abstract class SimpleNetClientPacketHandlerBase<T> : ISimpleNetClientPacketHandler
    {
        public void Handle(SimpleNetPacketBase packet, byte[] bytes)
        {
            this.Handle(ZeroFormatterSerializer.Deserialize<T>(bytes));
        }

        public Type GetPacketType()
        {
            return typeof(T);
        }

        public abstract void Handle(T packet);
    }
}