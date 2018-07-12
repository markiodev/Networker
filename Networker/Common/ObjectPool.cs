using System;
using System.Collections.Generic;

namespace Networker.Common
{
    public class ObjectPool<T>
    {
        public int Capacity { get; }
        readonly Stack<T> m_pool;

        public ObjectPool(int capacity)
        {
            this.Capacity = capacity;
            this.m_pool = new Stack<T>(capacity);
        }

        public int Count
        {
            get { return this.m_pool.Count; }
        }

        public T Pop()
        {
            lock(this.m_pool)
            {
                return this.m_pool.Pop();
            }
        }

        public void Push(T item)
        {
            if(item == null)
            {
                throw new ArgumentNullException("Items added to pool cannot be null");
            }

            lock(this.m_pool)
            {
                this.m_pool.Push(item);
            }
        }
    }
}