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

        public async Task Handle(byte[] packet, ISender sender)
        {
            await this.Process(this.PacketSerialiser.Deserialise<T>(packet, 0, 0), sender);
        }

        public async Task Handle(byte[] packet, int offset, int length, ISender sender)
        {
            await this.Process(this.PacketSerialiser.Deserialise<T>(packet, offset, length), sender);
        }

        public abstract Task Process(T packet, ISender sender);
    }
}