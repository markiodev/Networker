using System;

namespace Networker.Interfaces
{
    public interface IContainerIoc
    {
        void RegisterType<TService, TImplementation>() where TImplementation : TService;
        void RegisterType<TImplementation>();
        void RegisterSingleton<T>(T instance);
        void RegisterType(Type type);
        T Resolve<T>();
        T Resolve<T>(Type type);
        void VerifyResolutions();
    }
}