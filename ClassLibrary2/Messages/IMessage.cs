using System;

namespace ClassLibrary2.Messages
{
    public interface IMessage
    {
        Guid MessageId { get; }
        Guid CorrelationId { get; }
        Guid? CausationId { get; }

     

    }
}