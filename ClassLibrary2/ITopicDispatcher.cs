using System;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public interface ITopicDispatcher
    {
        void Publish<T>(string topic, T message) where T : class, IMessage;
        void Subscribe<T>(string topic, IHandler<T> handler) where T : class, IMessage;
    }
}