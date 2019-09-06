using System;
using Microsoft.Extensions.DependencyInjection;
using Networker.Events;

namespace Networker
{
    public interface IServerBuilder
    {
        IServer Build();
        IServerBuilder UseModule<T>() where T : IModuleBuilder;
        IServerBuilder UseModule(IModuleBuilder moduleBuilder);
        IServerBuilder UseTcp(int port);
        IServerBuilder UseUdp(int port);
        IServerBuilder SetMaxConnections(int maxConnections);
        IServerBuilder OnError(Action<Exception> action);
        IServerBuilder OnClientConnected(Action<ClientConnectedEvent> action);
        IServerBuilder OnClientDisconnected(Action<ClientDisconnectedEvent> action);
        IServerBuilder ConfigureServices(Action<IServiceCollection> configureFunc);
        IServerBuilder OnLog(Action<string, string> action);
    }
}