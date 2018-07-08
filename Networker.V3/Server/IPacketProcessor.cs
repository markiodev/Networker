using System.Net.Sockets;
using Networker.V3.Common;

namespace Networker.V3.Server
{
    public interface IPacketProcessor
    {

        void ProcessTcp(SocketAsyncEventArgs socketEvent);
        void ProcessUdp(SocketAsyncEventArgs socketEvent);
    }
}