using System;
using DryIoc;
using Networker.Interfaces;

namespace Networker.Common
{
    public class DryIocContainer : IContainerIoc
    {
        private readonly Container container;

        public DryIocContainer()
        {
            this.container = new Container();
        }

        public void RegisterType<TImplementation>()
        {
            this.container.Register<TImplementation>();
        }

        public void RegisterSingleton<T>(T instance)
        {
            this.container.RegisterInstance(instance);
        }

        public void RegisterType<TService, TImplementation>() where TImplementation : TService
        {
            this.container.Register<TService, TImplementation>();
        }

        public void RegisterType(Type type)
        {
            this.container.Register(type);
        }

        public T Resolve<T>()
        {
            return this.container.Resolve<T>();
        }

        public T Resolve<T>(Type type)
        {
            return this.container.Resolve<T>(type);
        }

        public void VerifyResolutions()
        {
            this.container.VerifyResolutions();
        }
    }
}