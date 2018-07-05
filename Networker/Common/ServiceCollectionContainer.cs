using System;
using Microsoft.Extensions.DependencyInjection;
using Networker.Interfaces;

namespace Networker.Common
{
    public class ServiceCollectionContainer : IContainerIoc
    {
        private readonly ServiceCollection serviceCollection;
        public IServiceProvider ServiceProvider;

        public ServiceCollectionContainer(ServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public IServiceProvider GetServiceProvider()
        {
            if(this.ServiceProvider == null)
                this.ServiceProvider = this.serviceCollection.BuildServiceProvider();

            return this.ServiceProvider;
        }

        public void RegisterSingleton<T>(T instance)
            where T: class
        {
            this.serviceCollection.AddSingleton<T>(instance);
        }

        public void RegisterSingleton<T>()
            where T: class
        {
            this.serviceCollection.AddSingleton<T>();
        }

        public void RegisterType<TService, TImplementation>()
            where TImplementation: class, TService where TService: class
        {
            this.serviceCollection.AddTransient<TService, TImplementation>();
        }

        public void RegisterType<TService, TImplementation>(IocReuse reuse)
            where TImplementation: class, TService where TService: class
        {
            if(reuse == IocReuse.Singleton)
            {
                this.serviceCollection.AddSingleton<TService, TImplementation>();
            }
        }

        public void RegisterType<TImplementation>(IocReuse reuse)
            where TImplementation: class
        {
            if(reuse == IocReuse.Singleton)
            {
                this.serviceCollection.AddSingleton<TImplementation>();
            }
        }

        public void RegisterType<TImplementation>()
            where TImplementation: class
        {
            this.serviceCollection.AddTransient<TImplementation>();
        }

        public void RegisterType(Type type)
        {
            this.serviceCollection.AddSingleton(type);
        }

        public T Resolve<T>()
        {
            return this.GetServiceProvider()
                       .GetService<T>();
        }

        public T Resolve<T>(Type type)
        {
            return this.GetServiceProvider()
                       .GetService<T>();
        }

        public void VerifyResolutions() { }
    }
}