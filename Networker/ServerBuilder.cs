using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Networker.Events;

namespace Networker
{
    public class ServerBuilder : IServerBuilder
    {
        private readonly List<IModuleBuilder> _moduleBuilders;
        private readonly IServiceCollection _serviceCollection;
        private readonly Server server;

        public ServerBuilder()
        {
            _serviceCollection = new ServiceCollection();
            _moduleBuilders = new List<IModuleBuilder>();
            server = new Server();
        }

        public IServer Build()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            foreach (var moduleBuilder in _moduleBuilders)
            {
                var module = moduleBuilder.Build(serviceProvider);
                server.RegisterModule(module);
            }

            return server;
        }

        public IServerBuilder UseModule<T>() where T : IModuleBuilder
        {
            return this;
        }

        public IServerBuilder UseModule(IModuleBuilder module)
        {
            foreach (var registeredType in module.GetRegisteredTypes())
                _serviceCollection.AddTransient(registeredType.Value);

            _moduleBuilders.Add(module);
            return this;
        }

        public IServerBuilder UseTcp(int port)
        {
            server.SetTcpPort(port);
            return this;
        }

        public IServerBuilder UseUdp(int port)
        {
            server.SetUdpPort(port);
            return this;
        }

        public IServerBuilder SetMaxConnections(int maxConnections)
        {
            server.SetMaxConnections(maxConnections);
            return this;
        }

        public IServerBuilder OnLog(Action<string, string> action)
        {
            server.SetOnLogAction(action);
            return this;
        }

        public IServerBuilder OnError(Action<Exception> action)
        {
            server.SetOnErrorAction(action);
            return this;
        }

        public IServerBuilder OnClientConnected(Action<ClientConnectedEvent> action)
        {
            server.SetOnClientConnected(action);
            return this;
        }

        public IServerBuilder OnClientDisconnected(Action<ClientDisconnectedEvent> action)
        {
            server.SetOnClientDisconnected(action);
            return this;
        }

        public IServerBuilder ConfigureServices(Action<IServiceCollection> configureFunc)
        {
            configureFunc.Invoke(_serviceCollection);
            return this;
        }
    }
}