using System;
using System.Collections.Generic;

namespace ClassLibrary2
{
    public class Cashier : IOrderHandler
    {
        private readonly IOrderHandler _orderHandler;
        private readonly Dictionary<Guid, Order> _ordersToBePaid = new Dictionary<Guid, Order>();
        
        public Cashier(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
            
        }

        public bool Handle(Order order)
        {
            _ordersToBePaid.Add(order.Id, order);

            return true;
        }

        public bool TryPay(Guid orderId)
        {
            Order orderToPay;
            if (_ordersToBePaid.TryGetValue(orderId, out orderToPay))
            {
                orderToPay.IsPaid = true;
                _ordersToBePaid.Remove(orderId);
                _orderHandler.Handle(orderToPay);
                return true;
            }

            return false;
        }
    }
}