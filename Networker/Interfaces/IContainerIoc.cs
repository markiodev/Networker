using System;

namespace Networker.Interfaces
{
    public interface IContainerIoc
    {
        void RegisterSingleton<T>(T instance);

        void RegisterType<TService, TImplementation>()
            where TImplementation: TService;

        void RegisterType<TImplementation>();
        void RegisterType(Type type);
        T Resolve<T>();
        T Resolve<T>(Type type);
        void VerifyResolutions();
        T TryResolve<T>();
    }
}