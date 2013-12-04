using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class Cashier : IHandler<QueueOrderForPayment>
    {
        private readonly ITopicDispatcher _dispatcher;
        private readonly ConcurrentDictionary<Guid, OrderMessage> _ordersToBePaid = new ConcurrentDictionary<Guid, OrderMessage>();
        
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
                var orderToPay = orderMessage.Order;
                orderToPay.SettleBill();
                OrderMessage removedOrder;
                _ordersToBePaid.TryRemove(orderId, out removedOrder);
                _dispatcher.Publish(new Paid(orderToPay, orderMessage.CorrelationId, orderMessage.MessageId));

                var origColour = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("$");
                Console.ForegroundColor = origColour;

                return true;
            }

           

            return false;
        }

        public bool Handle(QueueOrderForPayment message)
        {
            _ordersToBePaid.TryAdd(message.Order.Id, message);
            return true;
        }
    }
}