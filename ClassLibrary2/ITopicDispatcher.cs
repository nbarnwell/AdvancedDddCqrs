using System;

namespace ClassLibrary2
{
    public interface ITopicDispatcher
    {
        void Publish<T>(T message);
        void Subscribe<T>(IHandler<T> handler) where T : class, IMessage;
    }
}