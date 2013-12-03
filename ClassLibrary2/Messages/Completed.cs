using System;

namespace ClassLibrary2.Messages
{
    public class Completed : OrderMessage
    {
        protected Completed(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}