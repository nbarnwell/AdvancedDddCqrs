using System;
using System.Collections.Generic;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class Waiter
    {
        private readonly string _name;
        private readonly ITopicDispatcher _dispatcher;

        public Waiter(string name, ITopicDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _name = name;
            _dispatcher = dispatcher;
        }

        public void TakeOrder(int tableNumber, IEnumerable<OrderItem> orderItems, Guid id, bool dodgeyCustomer)
        {
            var order = new Order(tableNumber, id, _name, dodgeyCustomer);

            foreach (var item in orderItems)
            {
                order.AddItem(item);
            }

            _dispatcher.Publish(new OrderTaken(order, order.Id, null));
        }
    }
}