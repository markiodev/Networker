using System;
using System.Net.Sockets;
using Networker.V3.Common;

namespace Networker.V3.Server
{
    public class PacketProcessor : IPacketProcessor
    {
        private readonly IPacketDeserialiser packetDeserialiser;

        public PacketProcessor(IPacketDeserialiser packetDeserialiser)
        {
            this.packetDeserialiser = packetDeserialiser;
        }

        public void ProcessTcp(SocketAsyncEventArgs socketEvent) { }
        public void ProcessUdp(SocketAsyncEventArgs socketEvent) { }
    }
}