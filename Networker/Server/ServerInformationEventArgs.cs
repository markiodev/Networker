using System;

namespace Networker.Server
{
    public class ServerInformationEventArgs : EventArgs
    {
        public int ProcessedTcpPackets { get; set; }
        public int InvalidTcpPackets { get; set; }
        public int ProcessedUdpPackets { get; set; }
        public int InvalidUdpPackets { get; set; }
    }
}