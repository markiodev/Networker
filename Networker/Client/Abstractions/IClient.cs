using System;
using System.Net.Sockets;

namespace Networker.Client.Abstractions
{
    public interface IClient
    {
        EventHandler<Socket> Connected { get; set; }
        EventHandler<Socket> Disconnected { get; set; }

        void Send<T>(T packet);
        void Send(byte[] packet);

        void SendUdp<T>(T packet);
        void SendUdp(byte[] packet);

        long Ping(int timeout = 10000);

        void Connect();
        void Stop();
    }
}
