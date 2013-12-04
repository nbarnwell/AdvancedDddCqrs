using System;

namespace AdvancedDddCqrs.Messages
{
    public class OrderMessage : IMessage, IHaveTTL
    {
        private DateTime? _expiry;

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

        public bool HasExpired()
        {
            return DateTime.UtcNow > _expiry;
        }

        public void SetExpiry(TimeSpan duration)
        {
            if (_expiry == null)
            {
                _expiry = DateTime.UtcNow.Add(duration);
            }
        }
    }
}