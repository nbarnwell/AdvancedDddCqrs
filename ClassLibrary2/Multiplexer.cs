using System;

namespace ClassLibrary2
{
    public class Multiplexer : IOrderHandler
    {
        private readonly IOrderHandler[] _handlers;

        public Multiplexer(params IOrderHandler[] handlers)
        {
            if (handlers == null) throw new ArgumentNullException("handlers");
            _handlers = handlers;
        }

        public bool Handle(Order order)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(order);
            }

            return true;
        }
    }
}