using System;
using Networker.Common;
using Networker.Interfaces;
using ZeroFormatter;

namespace Networker.Server
{
    public abstract class ServerPacketHandlerBase<T> : IServerPacketHandler
    {
        public void Handle(INetworkerConnection clientConnection, NetworkerPacketBase packet, byte[] bytes)
        {
            this.Handle(clientConnection, ZeroFormatterSerializer.Deserialize<T>(bytes));
        }

        public abstract void Handle(INetworkerConnection sender, T packet);
    }
}