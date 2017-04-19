using System;

namespace Networker.Interfaces
{
    public interface IContainerIoc
    {
        void RegisterSingleton<T>(T instance);
        void RegisterType<T>();
        void RegisterType(Type type);
        T Resolve<T>();
        T Resolve<T>(Type type);
    }
}