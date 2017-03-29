namespace SimpleNet
{
    public interface ISimpleNetServerBuilder
    {
        ISimpleNetServerBuilder UseIpAddresses(string[] ipAddresses);
        ISimpleNetServerBuilder UsePort(int port);
        ISimpleNetServer Build<T>() where T : ISimpleNetServer;
        ISimpleNetServerBuilder UseTcp();
        ISimpleNetServerBuilder UseUdp();
        ISimpleNetServerBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>() where TPacketHandlerType : ISimpleNetServerPacketHandler<TPacketType>;
        ISimpleNetServerBuilder RegisterPacketHandlerModule<TPacketHandlerModule>();
    }
}