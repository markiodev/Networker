namespace Networker
{
    public interface IConnection
    {
        void Send(byte[] packet);
        void Send<T>(int packetType, T packet);
        void Send(int packetType, string packet);
        void Send(int packetType, byte[] packet);
    }
}
