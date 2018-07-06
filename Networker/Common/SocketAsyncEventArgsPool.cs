using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Networker.Common
{
    public class SocketAsyncEventArgsPool
    {
        readonly Stack<SocketAsyncEventArgs> m_pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            this.m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public int Count
        {
            get { return this.m_pool.Count; }
        }

        public SocketAsyncEventArgs Pop()
        {
            lock(this.m_pool)
            {
                return this.m_pool.Pop();
            }
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if(item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            lock(this.m_pool)
            {
                this.m_pool.Push(item);
            }
        }
    }
}