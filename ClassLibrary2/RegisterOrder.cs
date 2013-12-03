using System;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class RegisterOrder : OrderMessage
    {
        public RegisterOrder(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}