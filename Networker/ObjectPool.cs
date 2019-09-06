using System;
using System.Collections.Generic;

namespace Networker
{
    public abstract class ObjectPool<T>
    {
        private readonly Stack<T> m_pool;

        protected ObjectPool(int capacity)
        {
            Capacity = capacity;
            m_pool = new Stack<T>(capacity);
        }

        public int Capacity { get; }

        public int Count => m_pool.Count;
        protected abstract void OnRelease(T entity);

        public T Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        public void Push(T item)
        {
            if (item == null) throw new ArgumentNullException("Items added to pool cannot be null");

            lock (m_pool)
            {
                OnRelease(item);
                m_pool.Push(item);
            }
        }
    }
}