using System;
using Networker.Common.Abstractions;
using ZeroFormatter;

namespace Networker.Formatter.ZeroFormatter
{
    public class ZeroFormatterPacketIdentifierProvider : IPacketIdentifierProvider
    {
        public string Provide(byte[] packet)
        {
            var packetBase = ZeroFormatterSerializer.Deserialize<ZeroFormatterPacketBase>(packet);
            return packetBase.UniqueKey;
        }
    }
}