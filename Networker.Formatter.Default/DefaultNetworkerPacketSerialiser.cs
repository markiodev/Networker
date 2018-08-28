using System;
using Networker.Common.Abstractions;

namespace Networker.Formatter.Default
{
    public class DefaultNetworkerPacketSerialiser : IPacketSerialiser
    {
        public T Deserialise<T>(byte[] packetBytes, int offset, int length)
        {
            return default(T);
        }

        public byte[] Serialise<T>(T packet)
        {
            return new byte[] { };
        }

        public bool CanReadOffset => false;
    }
}
