using System;

namespace ClassLibrary2.Messages
{
    public class Paid : OrderMessage
    {
        public Paid(Order order, Guid  correlationId, Guid? causationId ) : base(order, correlationId, causationId)
        {
        }
    }
}