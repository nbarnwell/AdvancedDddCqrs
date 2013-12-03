using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public class Multiplexer<T> : IHandler<T> where T : class, IMessage
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
                //var newMessage = (T)message.clone();
                handler.Handle(message);
            }

            return true;
        }

        public void RemoveHandler<TMsg>(IHandler<TMsg> handlerToRemove) where TMsg : class, IMessage
        {
            var narrowingHandler = _handlers.OfType<NarrowingHandler<IMessage, TMsg>>()
                                            .SingleOrDefault(x => x.Handler == handlerToRemove);

            if (narrowingHandler != null)
            {
                _handlers.Remove((IHandler<T>)narrowingHandler);
            }
        }
    }
}