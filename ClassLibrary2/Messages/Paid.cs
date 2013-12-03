namespace ClassLibrary2.Messages
{
    public class Paid : OrderMessage
    {
        public Paid(Order order) : base(order)
        {
        }
    }
}