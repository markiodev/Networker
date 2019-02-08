using System;
using System.Net;

namespace Networker.Common.Abstractions
{
    public interface ISender
    {
        void Send<T>(T packet);
        IPEndPoint EndPoint { get; }
    }
}