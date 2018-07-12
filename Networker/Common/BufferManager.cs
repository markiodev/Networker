using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Networker.Common.Abstractions;

namespace Networker.Common
{
    public class BufferManager : IBufferManager
    {
        byte[] m_buffer;
        readonly int m_bufferSize;
        int m_currentIndex;
        readonly Stack<int> m_freeIndexPool;
        readonly int m_numBytes;

        public BufferManager(int totalBytes, int bufferSize)
        {
            this.m_numBytes = totalBytes;
            this.m_currentIndex = 0;
            this.m_bufferSize = bufferSize;
            this.m_freeIndexPool = new Stack<int>();
        }

        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            this.m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

        public void InitBuffer()
        {
            this.m_buffer = new byte[this.m_numBytes];
        }

        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if(this.m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(this.m_buffer, this.m_freeIndexPool.Pop(), this.m_bufferSize);
            }
            else
            {
                if((this.m_numBytes - this.m_bufferSize) < this.m_currentIndex)
                {
                    return false;
                }

                args.SetBuffer(this.m_buffer, this.m_currentIndex, this.m_bufferSize);
                this.m_currentIndex += this.m_bufferSize;
            }

            return true;
        }
    }
}