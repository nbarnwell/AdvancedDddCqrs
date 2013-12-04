using System;

namespace AdvancedDddCqrs.Messages
{
    public class LogMessage : IMessage
    {
        public Guid MessageId { get; private set; }
        public Guid CorrelationId { get; private set; }
        public Guid? CausationId { get; private set; }

        public string Message { get; set; }
        public ConsoleColor Color { get; set; }

        public LogMessage(IMessage initiatingMessage, string logMessage, ConsoleColor color)
        {
            MessageId     = Guid.NewGuid();
            CorrelationId = initiatingMessage.CorrelationId;
            CausationId   = initiatingMessage.MessageId;
            Message       = logMessage;
            Color         = color;
        }
    }
}