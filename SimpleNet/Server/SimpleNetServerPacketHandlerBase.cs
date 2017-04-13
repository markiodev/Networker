using System;
using SimpleNet.Common;
using SimpleNet.Interfaces;
using ZeroFormatter;

namespace SimpleNet.Server
{
    public abstract class SimpleNetServerPacketHandlerBase<T> : ISimpleNetServerPacketHandler
    {
        public Type GetPacketType()
        {
            return typeof(T);
        }

        public void Handle(ISimpleNetConnection clientConnection, SimpleNetPacketBase packet, byte[] bytes)
        {
            this.Handle(clientConnection, ZeroFormatterSerializer.Deserialize<T>(bytes));
        }

        public abstract void Handle(ISimpleNetConnection sender, T packet);
    }
}