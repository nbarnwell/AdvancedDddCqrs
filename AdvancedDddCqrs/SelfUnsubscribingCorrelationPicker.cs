using System;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class SelfUnsubscribingCorrelationPicker : IHandler<OrderTaken>
    {
        private readonly ITopicDispatcher _topicDispatcher;

        public SelfUnsubscribingCorrelationPicker(ITopicDispatcher topicDispatcher)
        {
            if (topicDispatcher == null) throw new ArgumentNullException("topicDispatcher");
            _topicDispatcher = topicDispatcher;
        }

        public bool Handle(OrderTaken message)
        {
            _topicDispatcher.Unsubscribe(message.CorrelationId.ToString(), this);

            _topicDispatcher.Subscribe(message.CorrelationId.ToString(), new Printer());

            return true;
        }
    }
}