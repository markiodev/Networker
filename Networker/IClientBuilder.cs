namespace Networker
{
    public interface IClientBuilder
    {
        IClientBuilder UseIp(string address);
        IClientBuilder UseTcp(int port);
        IClientBuilder UseUdp(int port);
        IClientBuilder UseModule(IModuleBuilder moduleBuilder);
        IClient Build();
    }
}