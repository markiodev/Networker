using System;

namespace Networker.Client
{
    public class ClientConfiguration
    {
        public string Ip { get; set; }
        public int TcpPort { get; set; }
        public int UdpPortLocal { get; set; }
        public int UdpPortRemote { get; set; }
        public bool UseTcp { get; set; }
        public bool UseUdp { get; set; }
    }
}