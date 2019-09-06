namespace Networker
{
    public interface IServer
    {
        void Start();
        void Stop();
        void RegisterModule(IModule module);
        int ActiveConnections { get; }
        ServerState State { get; set; }

        void Broadcast(byte[] packet);
        void Broadcast<T>(int packetType, T packet);
        void WriteLog(string logType, string message);

        IConnection GetConnection(int connectionId);
    }
}
