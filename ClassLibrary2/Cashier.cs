using System;
using System.Collections.Generic;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public class Cashier : IHandler<Priced>
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

        public bool Handle(Priced message)
        {
            _ordersToBePaid.Add(message.Order.Id, message);
            return true;
        }
    }
}