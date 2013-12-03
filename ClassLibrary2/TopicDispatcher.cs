using System;
using System.Collections.Generic;

namespace ClassLibrary2
{
    public class TopicDispatcher //: ITopicDispatcher
    {
        private readonly IDictionary<string, Multiplexer<IMessage>> _subscriptions = new Dictionary<string, Multiplexer<IMessage>>();

        public void Publish<T>(T message)
        {
            throw new NotImplementedException();
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