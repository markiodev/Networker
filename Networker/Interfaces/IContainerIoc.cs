using System;
using Networker.Common;

namespace Networker.Interfaces
{
    public interface IContainerIoc
    {
        void RegisterSingleton<T>(T instance);
        void RegisterSingleton<T>();

        void RegisterType<TService, TImplementation>()
            where TImplementation: TService;

        void RegisterType<TService, TImplementation>(IocReuse reuse)
            where TImplementation: TService;

        void RegisterType<TImplementation>(IocReuse reuse);

        void RegisterType<TImplementation>();
        void RegisterType(Type type);
        T Resolve<T>();
        T Resolve<T>(Type type);
        void VerifyResolutions();
    }
}