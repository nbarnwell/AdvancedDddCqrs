using System;
using System.Collections.Generic;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public class TopicDispatcher : ITopicDispatcher
    {
        private readonly IDictionary<string, Multiplexer<IMessage>> _subscriptions = new Dictionary<string, Multiplexer<IMessage>>();

        public void Publish<T>(string topic, T message) where T : class, IMessage
        {
            Multiplexer<IMessage> handlers;
            if (_subscriptions.TryGetValue(topic, out handlers))
            {
                handlers.Handle(message);
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
    }
}