using System;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public class CorrelationPicker : IHandler<OrderTaken>
    {
        private readonly ITopicDispatcher _topicDispatcher;
        private bool _finished = false;
        public CorrelationPicker(ITopicDispatcher topicDispatcher)
        {
            if (topicDispatcher == null) throw new ArgumentNullException("topicDispatcher");
            _topicDispatcher = topicDispatcher;
        }

        public bool Handle(OrderTaken message)
        {
            if (_finished)
            {
                return true;
            }

            _topicDispatcher.Subscribe(message.CorrelationId.ToString(), new Printer());
            _topicDispatcher.Unsubscribe(message.CorrelationId.ToString(), this);
            _finished = true;
            return true;
        }
    }
}