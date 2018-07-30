using System;
using Networker.Server.Abstractions;

namespace Networker.Server
{
    public class ServerInformation : IServerInformation
    {
        public ServerInformation()
        {
            this.IsRunning = true;
        }
        public int ProcessedTcpPackets { get; set; }
        public int ProcessedUdpPackets { get; set; }
        public int InvalidTcpPackets { get; set; }
        public int InvalidUdpPackets { get; set; }
        public bool IsRunning { get; set; }
    }
}