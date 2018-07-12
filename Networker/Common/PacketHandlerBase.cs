using System;
using System.Threading.Tasks;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public abstract class PacketHandlerBase<T> : IPacketHandler
        where T: PacketBase
    {
        public IPacketSerialiser PacketSerialiser { get; }

        protected PacketHandlerBase(IPacketSerialiser packetSerialiser)
        {
            this.PacketSerialiser = packetSerialiser;
        }

        public async Task Handle(byte[] packet, ISender sender)
        {
            await this.Process(this.PacketSerialiser.Deserialise<T>(packet), sender);
        }

        public abstract Task Process(T packet, ISender sender);
    }
}