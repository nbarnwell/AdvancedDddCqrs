using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AdvancedDddCqrs
{
    public class ThreadBoundary<T> : IHandler<T>, IDisposable
    {
        private BlockingCollection<T> _queue = new BlockingCollection<T>();

        public ThreadBoundary(IHandler<T> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            Task.Factory.StartNew(() =>
            {
                foreach (var order in _queue.GetConsumingEnumerable())
                {
                    handler.Handle(order);
                }
            });
        }

        public int QueueLength { get { return _queue.Count; } }

        public bool Handle(T message)
        {
            _queue.Add(message);

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