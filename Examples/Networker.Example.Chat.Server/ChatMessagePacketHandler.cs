using Networker.Example.Chat.Packets;
using Networker.Interfaces;
using Networker.Server;

namespace Networker.Example.Chat.Server
{
    public class ChatMessagePacketHandler : ServerPacketHandlerBase<ChatMessagePacket>
    {
        private readonly ITcpConnectionsProvider connectionsProvider;

        public ChatMessagePacketHandler(ITcpConnectionsProvider connectionsProvider)
        {
            this.connectionsProvider = connectionsProvider;
        }

        public override void Handle(INetworkerConnection sender, ChatMessagePacket packet)
        {
            foreach(var tcpConnection in this.connectionsProvider.Provide())
            {
                tcpConnection.Send(packet);
            }
        }
    }
}