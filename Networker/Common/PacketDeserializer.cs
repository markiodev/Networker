using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using ZeroFormatter;

namespace Networker.Common
{
    public class PacketDeserializer
    {
        public List<Tuple<NetworkerPacketBase, byte[]>> GetPacketsFromBuffer(byte[] packetBytes)
        {
            var packets = new List<Tuple<NetworkerPacketBase, byte[]>>();

            using (var memoryStream = new MemoryStream(packetBytes))
            {
                using (var readerStream = new BinaryReader(memoryStream))
                {
                    while (readerStream.BaseStream.Position < readerStream.BaseStream.Length)
                    {
                        var packetSize = readerStream.ReadInt32();

                        if (packetSize == 0)
                        {
                            break;
                        }

                        var bytes = readerStream.ReadBytes(packetSize);

                        var deserialized = ZeroFormatterSerializer.Deserialize<NetworkerPacketBase>(bytes);

                        if (string.IsNullOrEmpty(deserialized.UniqueKey))
                        {
                            throw new Exception("Invalid Packet.");
                        }

                        packets.Add(new Tuple<NetworkerPacketBase, byte[]>(deserialized, bytes));
                    }
                }
            }

            return packets;
        }
        public List<Tuple<NetworkerPacketBase, byte[]>> GetPacketsFromSocket(SocketAsyncEventArgs eventArgs)
        {
            var packets = new List<Tuple<NetworkerPacketBase, byte[]>>();

            using (var memoryStream = new MemoryStream(eventArgs.Buffer, eventArgs.Offset, eventArgs.BytesTransferred))
            {
                using (var readerStream = new BinaryReader(memoryStream))
                {
                    while (readerStream.BaseStream.Position < readerStream.BaseStream.Length)
                    {
                        var packetSize = readerStream.ReadInt32();

                        if (packetSize == 0)
                        {
                            break;
                        }

                        var bytes = readerStream.ReadBytes(packetSize);

                        var deserialized = ZeroFormatterSerializer.Deserialize<NetworkerPacketBase>(bytes);

                        if (string.IsNullOrEmpty(deserialized.UniqueKey))
                        {
                            throw new Exception("Invalid Packet.");
                        }

                        packets.Add(new Tuple<NetworkerPacketBase, byte[]>(deserialized, bytes));
                    }
                }
            }

            return packets;
        }

        public List<Tuple<NetworkerPacketBase, byte[]>> GetPacketsFromSocket(Socket socket)
        {
            var packets = new List<Tuple<NetworkerPacketBase, byte[]>>();

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

                    var deserialized = ZeroFormatterSerializer.Deserialize<NetworkerPacketBase>(bytes);

                    if(string.IsNullOrEmpty(deserialized.UniqueKey))
                    {
                        throw new Exception("Invalid Packet.");
                    }

                    packets.Add(new Tuple<NetworkerPacketBase, byte[]>(deserialized, bytes));
                }
            }

            return packets;
        }

        public List<Tuple<NetworkerPacketBase, byte[]>> GetPacketsFromUdp(UdpReceiveResult result)
        {
            var packets = new List<Tuple<NetworkerPacketBase, byte[]>>();

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

                    var deserialized = ZeroFormatterSerializer.Deserialize<NetworkerPacketBase>(bytes);

                    if(string.IsNullOrEmpty(deserialized.UniqueKey))
                    {
                        throw new Exception("Invalid Packet.");
                    }

                    packets.Add(new Tuple<NetworkerPacketBase, byte[]>(deserialized, bytes));
                }
            }

            return packets;
        }
    }
}