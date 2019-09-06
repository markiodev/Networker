using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Networker
{
    public class ClientBuilder : IClientBuilder
    {
        private readonly Client _client;
        private readonly List<IModuleBuilder> _moduleBuilders;
        private readonly IServiceCollection _serviceCollection;

        public ClientBuilder()
        {
            _moduleBuilders = new List<IModuleBuilder>();
            _serviceCollection = new ServiceCollection();
            _client = new Client();
        }

        public IClientBuilder UseIp(string address)
        {
            _client.SetIp(address);
            return this;
        }

        public IClientBuilder UseTcp(int port)
        {
            _client.SetTcpPort(port);
            return this;
        }

        public IClientBuilder UseUdp(int port)
        {
            return this;
        }

        public IClientBuilder UseModule(IModuleBuilder moduleBuilder)
        {
            foreach (var registeredType in moduleBuilder.GetRegisteredTypes())
            {
                _serviceCollection.AddTransient(registeredType.Value);
            }

            _moduleBuilders.Add(moduleBuilder);
            return this;
        }

        public IClient Build()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            foreach (var moduleBuilder in _moduleBuilders) _client.RegisterModule(moduleBuilder.Build(serviceProvider));

            return _client;
        }
    }
}