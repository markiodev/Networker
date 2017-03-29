using System;

namespace SimpleNet
{
    public interface ISimpleNetServer
    {
        ISimpleNetServer Start();
        void Stop();
    }
}
