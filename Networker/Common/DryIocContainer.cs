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

        public void RegisterSingleton<T>(T instance)
        {
            this.container.RegisterInstance(instance);
        }

        public void RegisterType<T>()
        {
            this.container.Register<T>();
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
    }
}