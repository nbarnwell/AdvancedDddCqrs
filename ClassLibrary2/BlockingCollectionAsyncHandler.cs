using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ClassLibrary2
{
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

        public bool Handle(Order order)
        {
            _queue.Add(order);

            return true;
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
}