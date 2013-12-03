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
            //Console.WriteLine("Now: {0}, _expiry: {1}", DateTime.UtcNow, _expiry);
            return DateTime.UtcNow > _expiry;
        }

        public void SetExpiry(TimeSpan duration)
        {
            if (_expiry == null)
            {
                _expiry = DateTime.UtcNow.Add(duration);
            }
            else
            {
                //Console.WriteLine("Resetting expiry!");
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