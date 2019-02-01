using System;

namespace Networker.Common.Abstractions
{
    public interface IPacketSerialiser
    {
        T Deserialise<T>(byte[] packetBytes);
        T Deserialise<T>(byte[] packetBytes, int offset, int length);
        byte[] Serialise<T>(T packet);
        byte[] Package(string name, byte[] bytes);
        bool CanReadOffset { get; }
        bool CanReadName { get; }
        bool CanReadLength { get; }
    }
}