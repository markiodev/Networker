using Networker.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Networker.Helpers;
using Networker.Interfaces;
using Networker.Example.Chat.Packets;

namespace Networker.Example.Chat.ClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private INetworkerClient client;

        //This is horrible, don't do this.
        public static MainWindow Instance { get; set; }

        public MainWindow()
        {
            Instance = this;
            this.InitializeComponent();

            this.client = new NetworkerClientBuilder().UseConsoleLogger()
                                                     .UseIp("127.0.0.1")
                                                     .UseTcp(1000)
                                                     .RegisterPacketHandler<ChatMessagePacket, ChatMessageReceivedPacketHandler>()
                                                     .Build<DefaultClient>()
                                                     .Connect();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.client.Send(new ChatMessagePacket
                             {
                                 Sender = Environment.MachineName,
                                 Message = this.MessageBox.Text
                             });
        }
    }
}
