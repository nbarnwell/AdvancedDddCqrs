using System.Collections.Generic;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class TopicDispatcher : ITopicDispatcher
    {
        private readonly IDictionary<string, Multiplexer<IMessage>> _subscriptions = new Dictionary<string, Multiplexer<IMessage>>();

        public void Publish<T>(T message) where T : class, IMessage
        {
            var typeName = message.GetType().FullName;

            var topics = new List<string> { typeName, message.CorrelationId.ToString() };

            foreach (var topic in topics)
            {
                Multiplexer<IMessage> handlers;
                if (_subscriptions.TryGetValue(topic, out handlers))
                {
                    handlers.Handle(message);
                }
            }
        }

        public void Subscribe<T>(string topic, IHandler<T> handler) where T : class, IMessage
        {
            var imessageshandler = new NarrowingHandler<IMessage, T>(handler);

            Multiplexer<IMessage> existingHandler;
            if (_subscriptions.TryGetValue(topic, out existingHandler))
            {
                var clone = existingHandler.Clone();
                clone.AddHandler(imessageshandler);
                _subscriptions[topic] = clone;
            }
            else
            {
                var handlers = new Multiplexer<IMessage>(new[] { imessageshandler });
                _subscriptions.Add(topic, handlers);
            }
        }

        public void Unsubscribe<T>(string topic, IHandler<T> handler) where T : class, IMessage
        {
            Multiplexer<IMessage> multiplexer;
            if (_subscriptions.TryGetValue(topic, out multiplexer))
            {
               multiplexer.RemoveHandler<T>(handler);
            }
        }
    }
}