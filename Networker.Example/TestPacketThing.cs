using System;
using Networker.Common;
using ZeroFormatter;

namespace Networker.V3.Example
{
    public class TestPacketThing : PacketBase
    {
        [Index(2)]
        public virtual string SomeString { get; set; }
    }
}