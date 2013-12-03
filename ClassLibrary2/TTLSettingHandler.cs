using System;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class TTLSettingHandler<T> : IHandler<T>
    {
        private readonly IHandler<T> _handler;
        private readonly int _durationSeconds;

        public TTLSettingHandler(IHandler<T> handler, int durationSeconds)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            _handler = handler;

            _durationSeconds = durationSeconds;
        }

        public bool Handle(T message)
        {
            var hasTTL = message as IHaveTTL;
            if (hasTTL != null)
            {
                hasTTL.SetExpiry(TimeSpan.FromSeconds(_durationSeconds));
            }

            _handler.Handle(message);
            return true;
        }
    }
}