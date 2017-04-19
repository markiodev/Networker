using System;
using System.Collections.Generic;

namespace Networker.Client
{
    public class ClientConfiguration
    {
        public ClientConfiguration()
        {
            this.PacketHandlers = new Dictionary<string, Type>();
            this.PacketHandlerModules = new List<Type>();
        }

        public string Ip { get; set; }
        public List<Type> PacketHandlerModules { get; set; }
        public Dictionary<string, Type> PacketHandlers { get; set; }
        public int TcpPort { get; set; }
        public int UdpPortLocal { get; set; }
        public int UdpPortRemote { get; set; }
        public bool UseTcp { get; set; }
        public bool UseUdp { get; set; }
    }
}