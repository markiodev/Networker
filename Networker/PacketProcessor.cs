using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Networker
{
    public class PacketProcessor
    {
        private readonly PacketContextObjectPool _packetContextObjectPool;
        private readonly IPacketHandler[] _packetHandlers;

        public PacketProcessor(Dictionary<int, IPacketHandler> packetHandlers, PacketContextObjectPool packetContextObjectPool)
        {
            _packetContextObjectPool = packetContextObjectPool;

            if (packetHandlers.Any())
            {
                _packetHandlers = new IPacketHandler[packetHandlers.Max(e => e.Key) + 1];

                foreach (var packetHandler in packetHandlers)
                {
                    _packetHandlers[packetHandler.Key] = packetHandler.Value;
                }
            }
        }

        public void Process(IPacketContext packetContext)
        {
            var packetType = Marshal.ReadInt32(packetContext.Packet);
            _packetHandlers[packetType].Handle(packetContext);
            _packetContextObjectPool.Push(packetContext);
        }
    }
}