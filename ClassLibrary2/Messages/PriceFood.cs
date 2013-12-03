using System;

namespace ClassLibrary2.Messages
{
    public class PriceFood : OrderMessage
    {
        public PriceFood(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}