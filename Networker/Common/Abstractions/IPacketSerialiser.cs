using System;

namespace Networker.Common.Abstractions
{
    public interface IPacketSerialiser
    {
        T Deserialise<T>(byte[] packetBytes);
        byte[] Serialise(PacketBase packet);
    }
}