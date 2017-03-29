namespace SimpleNet
{
    public interface ISimpleNetClientBuilder
    {
        ISimpleNetClientBuilder UseIp(string ip);
        ISimpleNetClientBuilder UsePort(int port);
        ISimpleNetClientBuilder UseTcp();
        ISimpleNetClientBuilder UseUdp();
        ISimpleNetClient Build();
        ISimpleNetClient Build<T>();
        ISimpleNetClientBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>() where TPacketHandlerType : ISimpleNetClientPacketHandler<TPacketType>;
        ISimpleNetClientBuilder RegisterPacketHandlerModule<TPacketHandlerModule>();
        ISimpleNetClientBuilder UseDryIoc();
    }
}