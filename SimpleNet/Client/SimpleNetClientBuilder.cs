namespace SimpleNet
{
    public class SimpleNetClientBuilder : ISimpleNetClientBuilder
    {
        public ISimpleNetClientBuilder UseIp(string ip)
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetClientBuilder UsePort(int port)
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetClientBuilder UseTcp()
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetClientBuilder UseUdp()
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetClient Build()
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetClient Build<T>()
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetClientBuilder RegisterPacketHandler<TPacketType, TPacketHandlerType>() where TPacketHandlerType : ISimpleNetClientPacketHandler<TPacketType>
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetClientBuilder RegisterPacketHandlerModule<TPacketHandlerModule>()
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetClientBuilder UseDryIoc()
        {
            throw new System.NotImplementedException();
        }
    }
}