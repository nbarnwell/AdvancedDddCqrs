namespace AdvancedDddCqrs
{
    public interface ISupportMemoisation<T>
    {
        OrderMemento GetMemento();
    }
}