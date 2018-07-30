using System;
using System.Threading.Tasks;

namespace Networker.Common.Abstractions
{
    public interface IPacketHandler
    {
        Task Handle(byte[] packet, ISender sender);
        Task Handle(byte[] packet, int offset, int length, ISender sender);
    }
}