using System;
using System.Threading.Tasks;

namespace Networker.Common.Abstractions
{
    public interface IPacketHandler
    {
        Task Handle(byte[] packet, ISender sender);
    }
}