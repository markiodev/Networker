using System;
using System.Reflection;
using System.Threading.Tasks;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public abstract class PacketHandlerBase<T> : PacketHandlerBase, IPacketHandler<T>
        where T: class
    {
		protected PacketHandlerBase(IPacketSerialiser packetSerialiser) : base(packetSerialiser)
        {
           
        }

        protected PacketHandlerBase() : base()
        {
            
        }

		public async override Task Handle(byte[] packet, IPacketContext context)
        {
            await this.Process(this.PacketSerialiser.Deserialise<T>(packet), context);
        }

        public async override Task Handle(byte[] packet, int offset, int length, IPacketContext context)
        {
            await this.Process(this.PacketSerialiser.Deserialise<T>(packet, offset, length), context);
        }

        public abstract Task Process(T packet, IPacketContext context);
    }

	public abstract class PacketHandlerBase : IPacketHandler
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

        public async virtual Task Handle(byte[] packetBytes, IPacketContext context)
        {
			object packet = this.PacketSerialiser.Deserialise<object>(packetBytes);
			MethodInfo method = GetType().GetMethod("Process", new Type[] { packet.GetType(), typeof(IPacketContext) } );
			await (Task) method.Invoke(this, new object[] { packet, context });
        }

        public async virtual Task Handle(byte[] packetBytes, int offset, int length, IPacketContext context)
        {
			object packet = this.PacketSerialiser.Deserialise<object>(packetBytes, offset, length);
			MethodInfo method = GetType().GetMethod("Process", new Type[] { packet.GetType(), typeof(IPacketContext) } );
			await (Task) method.Invoke(this, new object[] { packet, context });
        }
	}
}