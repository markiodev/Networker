using System;
using SimpleNet.Common;

namespace SimpleNet.Interfaces
{
    public interface ISimpleNetConnection
    {
        void Send<T>(T packet)
            where T: SimpleNetPacketBase;
    }
}