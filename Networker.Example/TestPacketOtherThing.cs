using System;
using Networker.Common;
using ZeroFormatter;

namespace Networker.V3.Example
{
    public class TestPacketOtherThing : PacketBase
    {
        [Index(2)]
        public virtual int SomeInt { get; set; }
    }
}