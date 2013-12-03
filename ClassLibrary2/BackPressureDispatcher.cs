using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedDddCqrs
{
    public class BackPressureDispatcher<T> : IHandler<T>
    {
        private readonly IEnumerable<ThreadBoundary<T>> _handlers;
        private readonly int _maxQueueLength;

        public BackPressureDispatcher(IEnumerable<ThreadBoundary<T>> handlers, int maxQueueLength)
        {
            if (handlers == null) throw new ArgumentNullException("handlers");

            _handlers = handlers;
            _maxQueueLength = maxQueueLength;
        }

        public bool Handle(T message)
        {
            var next = GetNextHandler();

            if (next == null)
            {
              //  Console.WriteLine(string.Format("Pushing back order {0}", order.Id));
                return false;
            }

            next.Handle(message);
            return true;
        }

        private ThreadBoundary<T> GetNextHandler()
        {
            return _handlers.Where(x => x.QueueLength < _maxQueueLength)
                            .OrderBy(x => x.QueueLength)
                            .FirstOrDefault();
        }
    }
}