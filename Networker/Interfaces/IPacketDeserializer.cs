using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Networker.Common
{
    public interface IPacketDeserializer
    {
        List<Tuple<NetworkerPacketBase, byte[]>> GetPacketsFromSocket(Socket socket);
        List<Tuple<NetworkerPacketBase, byte[]>> GetPacketsFromUdp(UdpReceiveResult result);
        NetworkerPacketBase Deserialize(byte[] decrypted);
    }
}