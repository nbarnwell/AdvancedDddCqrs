using System;

namespace AdvancedDddCqrs.Messages
{
    public class OrderTaken : OrderMessage, IHaveTTL
    {
        private DateTime? _expiry;

        public OrderTaken(Order order, Guid correlationId, Guid? causationId)
            : base(order, correlationId, causationId)
        {
        }

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

        //public OrderTaken Clone()
        //{
        //    var clone = new OrderTaken(Order.Clone());
        //    clone._expiry = _expiry;
        //    return clone;
        //}
    }
}