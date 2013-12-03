using System;
using System.Threading;

namespace ClassLibrary2
{
    public class RetryDispatcher : IOrderHandler
    {
        private readonly IOrderHandler _handler;

        public RetryDispatcher(IOrderHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            _handler = handler;
        }

        public bool Handle(Order order)
        {
            while (_handler.Handle(order) == false)
            {
                Thread.Sleep(1);
            }

            return true;
        }
    }
}