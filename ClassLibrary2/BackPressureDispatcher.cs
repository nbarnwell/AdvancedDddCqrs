using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ClassLibrary2
{
    public class BackPressureDispatcher : IOrderHandler
    {
        private readonly IEnumerable<BlockingCollectionAsyncHandler> _handlers;
        private int MaxQueueLength = 5;

        public BackPressureDispatcher(IEnumerable<BlockingCollectionAsyncHandler> handlers)
        {
            _handlers = handlers;
        }

        public bool Handle(Order order)
        {
            BlockingCollectionAsyncHandler next;
            while ((next = GetNextHandler()) == null)
            {
                Thread.Sleep(1);
            }

            next.Handle(order);

            return true;
        }

        private BlockingCollectionAsyncHandler GetNextHandler()
        {
            return _handlers.Where(x => x.QueueLength <= MaxQueueLength)
                            .OrderBy(x => x.QueueLength)
                            .FirstOrDefault();
        }
    }
}