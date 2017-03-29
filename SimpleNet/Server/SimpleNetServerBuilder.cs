namespace SimpleNet
{
    public class SimpleNetServerBuilder : ISimpleNetServerBuilder
    {
        public ISimpleNetServerBuilder UseIpAddresses(string[] ipAddresses)
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetServerBuilder UsePort(int port)
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetServer Build<T>() where T : ISimpleNetServer
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetServerBuilder UseTcp()
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetServerBuilder UseUdp()
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetServerBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>() where TPacketHandlerType : ISimpleNetServerPacketHandler<TPacketType>
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetServerBuilder RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            throw new System.NotImplementedException();
        }
    }
}