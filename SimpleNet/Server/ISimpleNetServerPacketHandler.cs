namespace SimpleNet
{
    public interface ISimpleNetServerPacketHandler<T>
    {
        void Handle(T packet);
    }
}