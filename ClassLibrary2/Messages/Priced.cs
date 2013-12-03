namespace ClassLibrary2.Messages
{
    public class Priced : OrderMessage
    {
        public Priced(Order order) : base(order)
        {
        }
    }
}