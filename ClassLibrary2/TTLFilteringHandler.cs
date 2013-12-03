using System;

namespace ClassLibrary2
{
    public class TTLFilteringHandler : IOrderHandler
    {
        private readonly IOrderHandler _handler;
        private readonly int _durationSeconds;

        public TTLFilteringHandler(IOrderHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            _handler = handler;
        }

        public bool Handle(Order order)
        {
            if (!order.HasExpired())
            {
                return _handler.Handle(order);
            }
            Console.WriteLine("Dropped order: {0}", order.TableNumber);
            return true;
        }
    }
}