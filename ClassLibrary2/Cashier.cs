using System;
using System.Collections.Generic;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class Cashier : IHandler<RegisterOrder>
    {
        private readonly ITopicDispatcher _dispatcher;
        private readonly Dictionary<Guid, OrderMessage> _ordersToBePaid = new Dictionary<Guid, OrderMessage>();
        
        public Cashier(ITopicDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _dispatcher = dispatcher;

        }

        public bool TryPay(Guid orderId)
        {
            OrderMessage orderMessage;
            if (_ordersToBePaid.TryGetValue(orderId, out orderMessage))
            {
                Order orderToPay = orderMessage.Order;
                orderToPay.IsPaid = true;
                _ordersToBePaid.Remove(orderId);
                _dispatcher.Publish(new Paid(orderToPay, orderMessage.CorrelationId, orderMessage.MessageId));
                return true;
            }

            return false;
        }

        public bool Handle(RegisterOrder message)
        {
            _ordersToBePaid.Add(message.Order.Id, message);
            return true;
        }
    }
}