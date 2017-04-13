using System;
using SimpleNet.Common;
using ZeroFormatter;

namespace SimpleNet.Example
{
    public class ServerInformationResponsePacket : SimpleNetPacketBase
    {
        [Index(0)]
        public virtual string MachineName { get; set; }
    }
}