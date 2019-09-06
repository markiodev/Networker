using System;

namespace Networker.PacketHandlers
{
    public class FunctionHandler : IPacketHandler
    {
        private readonly Action<IPacketContext> _handleFunction;

        public FunctionHandler(Action<IPacketContext> handleFunction)
        {
            _handleFunction = handleFunction;
        }

        public void Handle(IPacketContext packetContext)
        {
            _handleFunction.Invoke(packetContext);
        }
    }
}