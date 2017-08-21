using System;
using Networker.Common;
using Networker.Interfaces;
using ZeroFormatter;

namespace Networker.Client
{
    public abstract class PacketHandlerBase<T> : IClientPacketHandler
    {
        public Type GetPacketType()
        {
            return typeof(T);
        }

        public void Handle(NetworkerPacketBase packet, byte[] bytes)
        {
            this.Handle(ZeroFormatterSerializer.Deserialize<T>(bytes));
        }

        public abstract void Handle(T packet);
    }
}