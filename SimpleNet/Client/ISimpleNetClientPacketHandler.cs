namespace SimpleNet
{
    public interface ISimpleNetClientPacketHandler<T>
    {
        void Handle(ISimpleNetConnection connection, T packet);
    }
}