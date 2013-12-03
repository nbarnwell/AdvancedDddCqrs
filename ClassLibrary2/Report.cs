using System;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public class Report : OrderMessage
    {
        public Report(Order order, Guid correlationId, Guid? causationId = null) : base(order, correlationId, causationId)
        {
        }
    }
}