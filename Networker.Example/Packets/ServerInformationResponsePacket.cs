using System;
using Networker.Common;
using ZeroFormatter;

namespace Networker.Example.Packets
{
    public class ServerInformationResponsePacket : NetworkerPacketBase
    {
        [Index(0)]
        public virtual string MachineName { get; set; }
    }
}