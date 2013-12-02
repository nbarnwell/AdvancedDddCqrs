using System;
using System.Collections.Generic;

namespace ClassLibrary2
{
    public class Waiter
    {
        private readonly IOrderHandler _orderHandler;

        public Waiter(IOrderHandler orderHandler)
        {
            if (orderHandler == null) throw new ArgumentNullException("orderHandler");
            _orderHandler = orderHandler;
        }

        public void TakeOrder(int tableNumber, IEnumerable<OrderItem> orderItems, Guid id)
        {
            var order = new Order(tableNumber, id);

            foreach (var item in orderItems)
            {
                order.AddItem(item);
            }

            _orderHandler.Handle(order);
        }
    }
}