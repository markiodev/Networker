using System;
using Networker.Client;
using Networker.Example.Chat.Packets;

namespace Networker.Example.Chat.ClientWPF
{
    public class ChatMessageReceivedPacketHandler : PacketHandlerBase<ChatMessagePacket>
    {
        public override void Handle(ChatMessagePacket packet)
        {
            var window = MainWindow.Instance;

            window.Dispatcher.Invoke(() =>
                                     {
                                         window.MessageListBox.Items.Add($"{packet.Sender}: {packet.Message}");
                                     });
        }
    }
}