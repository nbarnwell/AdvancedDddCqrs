using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary2
{
    public class Multiplexer<T> : IHandler<T>
    {
        private readonly IList<IHandler<T>> _handlers = new List<IHandler<T>>();

        public Multiplexer(IEnumerable<IHandler<T>> handlers)
        {
            _handlers = handlers.ToList();
        }

        public void AddHandler(IHandler<T> newHandler)
        {
            _handlers.Add(newHandler);
        }

        public Multiplexer<T> Clone()
        {
            var clone = new Multiplexer<T>(_handlers);
            return clone;
        }

        public bool Handle(T message)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(message);
            }

            return true;
        }
    }
}