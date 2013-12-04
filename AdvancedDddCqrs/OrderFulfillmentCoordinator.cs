using System;
using System.Collections.Generic;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class OrderFulfillmentCoordinator : IHandler<OrderTaken>, IHandler<IMessage>, IHandler<Completed>
    {
        private readonly Dictionary<Guid, OrderFulfillment> _processes = new Dictionary<Guid, OrderFulfillment>();
        private readonly ITopicDispatcher _dispatcher;

        public OrderFulfillmentCoordinator(ITopicDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public bool Handle(OrderTaken message)
        {
            AddNewOrderFulfillment(message);
            return true;
        }

        public bool Handle(IMessage message)
        {
            OrderFulfillment orderFulfillment;
            if (_processes.TryGetValue(message.CorrelationId, out orderFulfillment))
            {
                orderFulfillment.Handle((dynamic)message);
            }

            return true;
        }

        private void AddNewOrderFulfillment(OrderTaken message)
        {
            OrderFulfillment orderFulfillment;
            if (!_processes.TryGetValue(message.CorrelationId, out orderFulfillment))
            {
                orderFulfillment = new OrderFulfillment(_dispatcher, message);
                _processes.Add(message.CorrelationId, orderFulfillment);
                _dispatcher.Subscribe<IMessage>(message.CorrelationId.ToString(), this);
            }
        }

        public bool Handle(Completed message)
        {
            OrderFulfillment orderFulfillment;
            if (!_processes.TryGetValue(message.CorrelationId, out orderFulfillment))
            {
                _dispatcher.Unsubscribe<IMessage>(message.CorrelationId.ToString(), this);
                _processes.Remove(message.CorrelationId);          
            }

            return true;
        }
    }
}