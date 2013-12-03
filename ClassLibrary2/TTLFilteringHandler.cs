using System;
using ClassLibrary2.Messages;
using Newtonsoft.Json;

namespace ClassLibrary2
{
    public class TTLFilteringHandler<T> : IHandler<T>
    {
        private readonly IHandler<T> _handler;
        private readonly int _durationSeconds;

        public TTLFilteringHandler(IHandler<T> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            _handler = handler;
        }

        public bool Handle(T message)
        {
            var hasTTL = message as IHaveTTL;
            if (hasTTL != null)
            {
                if (!hasTTL.HasExpired())
                {
                    return _handler.Handle(message);
                }

            }
            Console.WriteLine("****************** Dropped message: {0}", JsonConvert.SerializeObject(message, Formatting.Indented));
            return true;
        }
    }
}