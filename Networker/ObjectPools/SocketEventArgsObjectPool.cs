using System.Net.Sockets;

namespace Networker
{
    public class SocketEventArgsObjectPool : ObjectPool<SocketAsyncEventArgs>
    {
        public SocketEventArgsObjectPool(int capacity) : base(capacity)
        {
        }

        protected override void OnRelease(SocketAsyncEventArgs entity)
        {
        }
    }
}