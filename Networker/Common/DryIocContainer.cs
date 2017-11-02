using System;
using DryIoc;
using Networker.Interfaces;

namespace Networker.Common
{
    public class DryIocContainer : IContainerIoc
    {
        public readonly Container Container;

        public DryIocContainer()
        {
            this.Container = new Container();
        }

        public void RegisterSingleton<T>(T instance)
        {
            this.Container.RegisterInstance(instance);
        }

        public void RegisterType<TImplementation>()
        {
            this.Container.Register<TImplementation>();
        }

        public void RegisterType<TService, TImplementation>()
            where TImplementation: TService
        {
            this.Container.Register<TService, TImplementation>();
        }

        public void RegisterType(Type type)
        {
            this.Container.Register(type);
        }

        public T Resolve<T>()
        {
            return this.Container.Resolve<T>();
        }

        public T Resolve<T>(Type type)
        {
            return this.Container.Resolve<T>(type);
        }

        public void VerifyResolutions()
        {
            this.Container.VerifyResolutions();
        }
    }
}