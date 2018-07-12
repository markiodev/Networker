using System;
using System.Net.Sockets;

namespace Networker.Common.Abstractions
{
    public interface IBufferManager
    {
        void FreeBuffer(SocketAsyncEventArgs args);
        void InitBuffer();
        bool SetBuffer(SocketAsyncEventArgs args);
    }
}