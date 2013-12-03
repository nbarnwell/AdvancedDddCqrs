using System;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class Report : OrderMessage
    {
        public Report(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}