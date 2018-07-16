namespace Networker.Common.Abstractions
{
    public interface IPacketIdentifierProvider
    {
        string Provide(byte[] packet);
    }
}