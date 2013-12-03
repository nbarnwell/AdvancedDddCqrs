using System;

namespace ClassLibrary2.Messages
{
    public class Cooked : OrderMessage
    {
        public Cooked(Order order, Guid correlationId, Guid? causationId)
            : base(order, correlationId, causationId)
        {
        }
    }
}