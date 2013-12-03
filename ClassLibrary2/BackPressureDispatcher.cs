using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary2
{
    public class BackPressureDispatcher : IOrderHandler
    {
        private readonly IEnumerable<ThreadBoundary> _handlers;
        private readonly int _maxQueueLength;

        public BackPressureDispatcher(IEnumerable<ThreadBoundary> handlers, int maxQueueLength)
        {
            if (handlers == null) throw new ArgumentNullException("handlers");

            _handlers = handlers;
            _maxQueueLength = maxQueueLength;
        }

        public bool Handle(Order order)
        {
            var next = GetNextHandler();

            if (next == null)
            {
              //  Console.WriteLine(string.Format("Pushing back order {0}", order.Id));
                return false;
            }

            next.Handle(order);
            return true;
        }

        private ThreadBoundary GetNextHandler()
        {
            return _handlers.Where(x => x.QueueLength < _maxQueueLength)
                            .OrderBy(x => x.QueueLength)
                            .FirstOrDefault();
        }
    }
}