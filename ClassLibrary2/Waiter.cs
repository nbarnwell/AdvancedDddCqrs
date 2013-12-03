using System;
using System.Collections.Generic;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public class Waiter
    {
        private readonly ITopicDispatcher _dispatcher;

        public Waiter(ITopicDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _dispatcher = dispatcher;
        }

        public void TakeOrder(int tableNumber, IEnumerable<OrderItem> orderItems, Guid id)
        {
            var order = new Order(tableNumber, id);

            foreach (var item in orderItems)
            {
                order.AddItem(item);
            }

            _dispatcher.Publish(new OrderTaken(order, order.Id, null));
        }
    }
}