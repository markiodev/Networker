using System;
using System.Threading.Tasks;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public abstract class PacketHandlerBase<T> : IPacketHandler
        where T: class
    {
        public IPacketSerialiser PacketSerialiser { get; }

        protected PacketHandlerBase(IPacketSerialiser packetSerialiser)
        {
            this.PacketSerialiser = packetSerialiser;
        }

        protected PacketHandlerBase()
        {
            this.PacketSerialiser = PacketSerialiserProvider.Provide();
        }

        public async Task Handle(byte[] packet, IPacketContext context)
        {
            await this.Process(this.PacketSerialiser.Deserialise<T>(packet), context);
        }

        public async Task Handle(byte[] packet, int offset, int length, IPacketContext context)
        {
            await this.Process(this.PacketSerialiser.Deserialise<T>(packet, offset, length), context);
        }

        public abstract Task Process(T packet, IPacketContext context);
    }
}