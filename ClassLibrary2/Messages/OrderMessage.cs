namespace ClassLibrary2.Messages
{
    public class OrderMessage : IMessage
    {
        public Order Order { get; private set; }

        public OrderMessage(Order order)
        {
            Order = order;
        }
    }
}