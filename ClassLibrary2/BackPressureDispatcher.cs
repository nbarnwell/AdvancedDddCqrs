using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary2
{
    public class BackPressureDispatcher : IOrderHandler
    {
        private readonly IEnumerable<BlockingCollectionAsyncHandler> _handlers;
        private readonly int _maxQueueLength;

        public BackPressureDispatcher(IEnumerable<IOrderHandler> handlers, int maxQueueLength)
        {
            if (handlers == null) throw new ArgumentNullException("handlers");

            _handlers = handlers;
            _maxQueueLength = maxQueueLength;
        }

        public bool Handle(Order order)
        {
            var next = GetNextHandler();

            if (next == null) return false;

            next.Handle(order);
            return true;
        }

        private BlockingCollectionAsyncHandler GetNextHandler()
        {
            return _handlers.Where(x => x.QueueLength < _maxQueueLength)
                            .OrderBy(x => x.QueueLength)
                            .FirstOrDefault();
        }
    }
}