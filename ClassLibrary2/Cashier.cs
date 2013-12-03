using System;
using System.Collections.Generic;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public class Cashier : IHandler<Priced>
    {
        private readonly ITopicDispatcher _dispatcher;
        private readonly Dictionary<Guid, Order> _ordersToBePaid = new Dictionary<Guid, Order>();
        
        public Cashier(ITopicDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _dispatcher = dispatcher;

        }

        public bool TryPay(Guid orderId)
        {
            Order orderToPay;
            if (_ordersToBePaid.TryGetValue(orderId, out orderToPay))
            {
                orderToPay.IsPaid = true;
                _ordersToBePaid.Remove(orderId);
                _dispatcher.Publish(typeof(Paid).FullName, new Paid(orderToPay));
                return true;
            }

            return false;
        }

        public bool Handle(Priced message)
        {
            _ordersToBePaid.Add(message.Order.Id, message.Order);
            return true;
        }
    }
}