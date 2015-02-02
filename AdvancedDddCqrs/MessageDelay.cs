using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class MessageDelay : IHandler<IMessage>
    {
        private readonly ITopicDispatcher _dispatcher;
        private readonly List<EchoWrapper> _messagesToEcho = new List<EchoWrapper>();

        public bool Cancelled { get; set; }

        public MessageDelay(ITopicDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _dispatcher = dispatcher;

            Task.Factory.StartNew(() =>
            {
                while (!Cancelled)
                {
                    var messages = _messagesToEcho.Where(x => x.HasExpired());

                    foreach (var message in messages)
                    {
                        _dispatcher.Publish((dynamic)message.Echo.Inner);
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        public bool Handle(IMessage message)
        {
            var echoWrapper = new EchoWrapper(message);
            echoWrapper.SetExpiry(message.Delay);
            _messagesToEcho.Add(echoWrapper);

            return true;
        }
    }

    internal class EchoWrapper
    {
        public EchoWrapper(IMessage message)
        {
            throw new NotImplementedException();
        }

        public bool HasExpired()
        {
            throw new NotImplementedException();
        }

        public object Echo { get; private set; }

        public void SetExpiry(TimeSpan delay)
        {
            throw new NotImplementedException();
        }
    }

    public class Echo<T> : IMessage
    {
        public Guid MessageId { get; private set; }
        public Guid CorrelationId { get; private set; }
        public Guid? CausationId { get; private set; }

        public int RetryCount { get; set; }
        public int MaxRetries { get; set; }
        public TimeSpan Delay { get; private set; }
        public T Inner { get; private set; }
    }
}