using System;
using AdvancedDddCqrs.Messages;

namespace AdvancedDddCqrs
{
    public class Logger : IHandler<LogMessage>
    {
        public bool Handle(LogMessage message)
        {
            var prevColor = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = message.Color;
            }
            finally
            {
                Console.ForegroundColor = prevColor;
            }

            return true;
        }
    }
}