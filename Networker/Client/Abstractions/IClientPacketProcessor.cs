using System;
using System.Net.Sockets;

namespace Networker.Client.Abstractions
{
    public interface IClientPacketProcessor
    {
        void Process(Socket socket);
        void Process(UdpReceiveResult data);
    }
}