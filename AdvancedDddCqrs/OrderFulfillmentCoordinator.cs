using System;
using System.Collections.Generic;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class OrderFulfillmentCoordinator : IHandler<OrderTaken>
    {
        private readonly Dictionary<Guid, OrderFulfillment> _processes = new Dictionary<Guid, OrderFulfillment>();
        private readonly ITopicDispatcher _dispatcher;

        public OrderFulfillmentCoordinator(ITopicDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public bool Handle(OrderTaken message)
        {
            var orderFulfillment = new OrderFulfillment(_dispatcher, message);
            _processes.Add(message.CorrelationId, orderFulfillment);
            _dispatcher.Subscribe<Cooked>(message.CorrelationId.ToString(), orderFulfillment);
            _dispatcher.Subscribe<Priced>(message.CorrelationId.ToString(), orderFulfillment);
            _dispatcher.Subscribe<Paid>(message.CorrelationId.ToString(), orderFulfillment);
            _dispatcher.Subscribe<Completed>(message.CorrelationId.ToString(), orderFulfillment);

            return true;
        }
    }
}