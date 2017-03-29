using SimpleNet.Client;

namespace SimpleNet
{
    public abstract class SimpleNetClientBase : ISimpleNetClient
    {
        private readonly string _ip;
        private readonly int _port;

        protected SimpleNetClientBase(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public ISimpleNetClient Connect()
        {
            throw new System.NotImplementedException();
        }

        public ISimpleNetClientPacketReceipt Send(ISimpleNetPacket packet)
        {
            throw new System.NotImplementedException();
        }
    }
}