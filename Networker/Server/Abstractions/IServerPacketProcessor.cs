using Networker.Common.Abstractions;
using System.Net;
using System.Net.Sockets;

namespace Networker.Server.Abstractions
{
    public interface IServerPacketProcessor
    {
        void SetUdpSender(IUdpSocketSender sender);
        void ProcessFromBuffer(ISender sender, byte[] buffer, int offset = 0, int length = 0, bool isTcp = true);
        void ProcessTcp(SocketAsyncEventArgs socketEvent);
        void ProcessUdp(SocketAsyncEventArgs socketEvent);
        void ProcessUdpFromBuffer(EndPoint sender, byte[] buffer, int offset = 0, int length = 0);
    }
}
