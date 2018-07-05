using System;
using DryIoc;
using Networker.Common;
using Networker.Interfaces;

namespace Networker.DryIoc
{
    public class DryIocContainer : IContainerIoc
    {
        public readonly Container Container;

        public DryIocContainer()
        {
            this.Container = new Container();
        }

        public void RegisterSingleton<T>(T instance)
            where T: class
        {
            this.Container.RegisterInstance(instance);
        }

        public void RegisterSingleton<T>()
            where T: class
        {
            this.Container.Register<T>(Reuse.Singleton);
        }

        public void RegisterType<TImplementation>()
            where TImplementation: class
        {
            this.Container.Register<TImplementation>();
        }

        public void RegisterType<TService, TImplementation>()
            where TImplementation: class, TService where TService: class
        {
            this.Container.Register<TService, TImplementation>();
        }

        public void RegisterType<TService, TImplementation>(IocReuse reuse)
            where TImplementation: class, TService where TService: class
        {
            switch(reuse)
            {
                case IocReuse.Singleton:
                    this.Container.Register<TService, TImplementation>(Reuse.Singleton);
                    break;
                default:
                    this.Container.Register<TService, TImplementation>();
                    break;
            }
        }

        public void RegisterType<TImplementation>(IocReuse reuse)
            where TImplementation: class
        {
            switch(reuse)
            {
                case IocReuse.Singleton:
                    this.Container.Register<TImplementation>(Reuse.Singleton);
                    break;
                default:
                    this.Container.Register<TImplementation>();
                    break;
            }
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