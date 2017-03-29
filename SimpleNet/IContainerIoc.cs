namespace SimpleNet
{
    public interface IContainerIoc
    {
        T Resolve<T>();
        void RegisterType<T>();
        void RegisterSingleton<T>(object instance);
    }
}