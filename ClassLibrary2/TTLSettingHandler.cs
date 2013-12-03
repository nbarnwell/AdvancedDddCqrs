using System;

namespace ClassLibrary2
{
    public class TTLSettingHandler : IOrderHandler
    {
        private readonly IOrderHandler _handler;
        private readonly int _durationSeconds;

        public TTLSettingHandler(IOrderHandler handler, int durationSeconds)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            _handler = handler;

            _durationSeconds = durationSeconds;
        }

        public bool Handle(Order order)
        {
            order.SetExpiry(TimeSpan.FromSeconds(_durationSeconds));
            _handler.Handle(order);
            return true;
        }
    }
}