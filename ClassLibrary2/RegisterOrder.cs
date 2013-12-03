using System;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public class RegisterOrder : OrderMessage
    {
        public RegisterOrder(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}