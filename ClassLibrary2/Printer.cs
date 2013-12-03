using System;
using ClassLibrary2.Messages;
using Newtonsoft.Json;

namespace ClassLibrary2
{
    class Printer : IHandler<IMessage>
    {
        public bool Handle(IMessage message)
        {
            Console.WriteLine("{0}: {1}", message,JsonConvert.SerializeObject(message, Formatting.Indented));
            return true;
        }
    }
}