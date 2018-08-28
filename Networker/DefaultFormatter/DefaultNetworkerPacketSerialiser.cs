using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Networker.Common.Abstractions;

namespace Networker.DefaultFormatter
{
    public class DefaultNetworkerPacketSerialiser : IPacketSerialiser
    {
        public bool CanReadOffset => false;
        public bool CanReadName => true;
        public bool CanReadLength => true;

        public T Deserialise<T>(byte[] packetBytes, int offset, int length)
        {
            throw new NotImplementedException();
            return (T)Activator.CreateInstance(typeof(T), packetBytes);
        }

        public byte[] Serialise<T>(T packet)
        {
            throw new NotImplementedException();
            BinaryFormatter bf = new BinaryFormatter();
            using(var ms = new MemoryStream())
            {
                bf.Serialize(ms, packet);
                return ms.ToArray();
            }
        }
    }
}