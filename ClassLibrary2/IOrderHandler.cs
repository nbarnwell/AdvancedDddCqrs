namespace ClassLibrary2
{
    public interface IHandler<T>
    {
        bool Handle(T message);
    }
    public interface IOrderHandler
    {
        bool Handle(Order order);
    }
}