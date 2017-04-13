using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using ZeroFormatter;

namespace SimpleNet.Common
{
    public class PacketDeserializer
    {
        public List<Tuple<SimpleNetPacketBase, byte[]>> GetPacketsFromSocket(Socket socket)
        {
            var packets = new List<Tuple<SimpleNetPacketBase, byte[]>>();

            var buffer = new byte[socket.Available];
            socket.Receive(buffer);

            using(var memoryStream = new MemoryStream(buffer))
            using(var readerStream = new BinaryReader(memoryStream))
            {
                while(readerStream.BaseStream.Position < readerStream.BaseStream.Length)
                {
                    var packetSize = readerStream.ReadInt32();

                    if(packetSize == 0)
                    {
                        break;
                    }

                    var bytes = readerStream.ReadBytes(packetSize);

                    var deserialized = ZeroFormatterSerializer.Deserialize<SimpleNetPacketBase>(bytes);

                    if(string.IsNullOrEmpty(deserialized.UniqueKey))
                    {
                        throw new Exception("Invalid Packet.");
                    }

                    packets.Add(new Tuple<SimpleNetPacketBase, byte[]>(deserialized, bytes));
                }
            }

            return packets;
        }

        public List<Tuple<SimpleNetPacketBase, byte[]>> GetPacketsFromUdp(UdpReceiveResult result)
        {
            var packets = new List<Tuple<SimpleNetPacketBase, byte[]>>();

            using(var memoryStream = new MemoryStream(result.Buffer))
            using(var readerStream = new BinaryReader(memoryStream))
            {
                while(readerStream.BaseStream.Position < readerStream.BaseStream.Length)
                {
                    var packetSize = readerStream.ReadInt32();

                    if(packetSize == 0)
                    {
                        break;
                    }

                    var bytes = readerStream.ReadBytes(packetSize);

                    var deserialized = ZeroFormatterSerializer.Deserialize<SimpleNetPacketBase>(bytes);

                    if(string.IsNullOrEmpty(deserialized.UniqueKey))
                    {
                        throw new Exception("Invalid Packet.");
                    }

                    packets.Add(new Tuple<SimpleNetPacketBase, byte[]>(deserialized, bytes));
                }
            }

            return packets;
        }
    }
}