using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class MessageDelay : IHandler<Echo>
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
                        _dispatcher.Publish(message.Inner.Inner);
                    }

                    Thread.Sleep(1000);
                }
            });
        }

        public bool Handle(Echo message)
        {
            var echoWrapper = new EchoWrapper(message);
            echoWrapper.SetExpiry(message.Delay);
            _messagesToEcho.Add(echoWrapper);

            return true;
        }
    }

    internal class EchoWrapper : IHaveTTL
    {
        private DateTime _expiry;

        public Echo Inner { get; private set; }

        public EchoWrapper(Echo message)
        {
            Inner = message;
        }

        public bool HasExpired()
        {
            return DateTime.UtcNow >= _expiry;
        }

        public void SetExpiry(TimeSpan duration)
        {
            _expiry = DateTime.UtcNow.Add(duration);
        }
    }

    public class Echo : IMessage
    {
        public Guid MessageId { get; private set; }
        public Guid CorrelationId { get; private set; }
        public Guid? CausationId { get; private set; }

        public TimeSpan Delay { get; private set; }
        public IMessage Inner { get; private set; }
    }
}