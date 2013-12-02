using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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

        public void Handle(Order order)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(order);
            }
        }
    }

    public class BlockingCollectionAsyncHandler : IOrderHandler, IDisposable
    {
        private BlockingCollection<Order> _queue = new BlockingCollection<Order>();

        public BlockingCollectionAsyncHandler(IOrderHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            Task.Factory.StartNew(() =>
            {
                foreach (var order in _queue.GetConsumingEnumerable())
                {
                    handler.Handle(order);
                }

                //Task.Factory.StartNew(() =>
                //{
                //    Parallel.ForEach(_queue.GetConsumingEnumerable(), handler.Handle);
                //});
            });
        }

        public int QueueLength { get { return _queue.Count; } }

        public void Handle(Order order)
        {
            _queue.Add(order);
        }

        public void Dispose()
        {
           if (_queue != null)
           {
               _queue.CompleteAdding();
               _queue = null;
           }
        }
    }

    public class TaskAsyncHandler : IOrderHandler
    {
        private readonly IOrderHandler _handler;

        public TaskAsyncHandler(IOrderHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            _handler = handler;
        }

        public void Handle(Order order)
        {
            Task.Factory.StartNew(() => _handler.Handle(order));
        }
    }
}