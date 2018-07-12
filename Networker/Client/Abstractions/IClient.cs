using System;
using System.Net.Sockets;
using Networker.Common;

namespace Networker.Client.Abstractions
{
    public interface IClient
    {
        EventHandler<Socket> Connected { get; set; }
        EventHandler<Socket> Disconnected { get; set; }
        void Connect();
        void Send(PacketBase packet);
        void SendUdp(PacketBase packet);
        int Ping();
        void Stop();
    }
}