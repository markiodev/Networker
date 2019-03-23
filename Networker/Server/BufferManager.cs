using System.Collections.Generic;
using System.Net.Sockets;
using Networker.Server.Abstractions;

namespace Networker.Server
{
	public class BufferManager : IBufferManager
	{
		private readonly int m_bufferSize;
		private readonly Stack<int> m_freeIndexPool;
		private readonly int m_numBytes;
		private byte[] m_buffer;
		private int m_currentIndex;

		public BufferManager(int totalBytes, int bufferSize)
		{
			m_numBytes = totalBytes;
			m_currentIndex = 0;
			m_bufferSize = bufferSize;
			m_freeIndexPool = new Stack<int>();
		}

		public void FreeBuffer(SocketAsyncEventArgs args)
		{
			m_freeIndexPool.Push(args.Offset);
			args.SetBuffer(null, 0, 0);
		}

		public void InitBuffer()
		{
			m_buffer = new byte[m_numBytes];
		}

		public bool SetBuffer(SocketAsyncEventArgs args)
		{
			if (m_freeIndexPool.Count > 0)
			{
				args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
			}
			else
			{
				if (m_numBytes - m_bufferSize < m_currentIndex) return false;

				args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
				m_currentIndex += m_bufferSize;
			}

			return true;
		}
	}
}