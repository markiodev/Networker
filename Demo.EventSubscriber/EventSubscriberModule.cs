using System.Collections.Generic;
using Networker;

namespace Demo.EventSubscriber
{
    public class EventSubscriberModule : IModule
    {
        public Dictionary<int, IPacketHandler> PacketHandlers => throw new System.NotImplementedException();

        public void Register(IModuleBuilder moduleBuilder)
        {
            //moduleBuilder.RegisterHandler<EventSubscriberReadyHandler>((int)PacketIdentifiers.SubscriberReady);
        }
    }
}