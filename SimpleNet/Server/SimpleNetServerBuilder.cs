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
    }
}