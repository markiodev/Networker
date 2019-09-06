namespace Networker
{
    public interface IClient
    {
        ConnectResult Connect();

        ConnectionState ConnectionState { get; }

        long Ping(int timeout);

        void Send(byte[] packet);
        
        void Send(int packetIdentifier, byte[] packet);

        void RegisterModule(IModule module);
    }
}