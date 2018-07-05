using System;
using Networker.Common;

namespace Networker.Interfaces
{
    public interface IContainerIoc
    {
        void RegisterSingleton<T>(T instance)
            where T : class;
        void RegisterSingleton<T>()
            where T : class;

        void RegisterType<TService, TImplementation>()
            where TImplementation : class, TService where TService : class;

        void RegisterType<TService, TImplementation>(IocReuse reuse)
            where TImplementation : class, TService where TService : class;

        void RegisterType<TImplementation>(IocReuse reuse)
            where TImplementation : class;

        void RegisterType<TImplementation>()
            where TImplementation : class;
        void RegisterType(Type type);
        T Resolve<T>();
        T Resolve<T>(Type type);
        void VerifyResolutions();
    }
}