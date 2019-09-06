using System;
using System.Runtime.InteropServices;

namespace Networker
{
    public class TcpConnection : IConnection
    {
        public SocketToken SocketToken { get; set; }

        public int Id { get; set; }

        public void Send(byte[] packet)
        {
            SocketToken.Socket.Send(packet);
        }

        public void Send<T>(int packetType, T packet)
        {
            throw new System.NotImplementedException();
        }

        public void Send(int packetType, string packet)
        {
            throw new System.NotImplementedException();
        }

        public void Send(int packetType, byte[] packet)
        {
            unsafe
            {
                var newEventBytes = Marshal.AllocHGlobal(packet.Length + 4);
                var newEventBytesSpan = new Span<byte>(newEventBytes.ToPointer(), packet.Length + 4);
                BitConverter.GetBytes(packetType).CopyTo(newEventBytesSpan.Slice(0, 4));
                packet.CopyTo(newEventBytesSpan.Slice(4, packet.Length));

                Send(newEventBytesSpan.ToArray());

                Marshal.FreeHGlobal(newEventBytes);
            }
        }
    }
}