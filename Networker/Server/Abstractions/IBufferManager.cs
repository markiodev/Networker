using System.Net.Sockets;

namespace Networker.Server.Abstractions
{
    public interface IBufferManager
    {
        void FreeBuffer(SocketAsyncEventArgs args);
        void InitBuffer();
        bool SetBuffer(SocketAsyncEventArgs args);
    }
}
