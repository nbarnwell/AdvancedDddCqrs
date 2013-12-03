using System;

namespace ClassLibrary2.Messages
{
    public class CookFood : OrderMessage
    {
        public CookFood(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}