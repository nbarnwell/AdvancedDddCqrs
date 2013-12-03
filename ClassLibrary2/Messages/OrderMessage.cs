using System;

namespace ClassLibrary2.Messages
{
    public class OrderMessage : IMessage
    {
        public Order Order { get; private set; }

        protected OrderMessage(Order order, Guid correlationId, Guid? causationId=null )
        {
            MessageId = Guid.NewGuid();
            CorrelationId = correlationId;
            if (causationId.HasValue)
            {
                CausationId = causationId;
            }
            
            Order = order;
        }

        public Guid MessageId { get; private set; }
        public Guid CorrelationId { get; private set; }
        public Guid? CausationId { get; private set; }
    }
}