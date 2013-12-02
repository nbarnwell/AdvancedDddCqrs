using System;
using Newtonsoft.Json;

namespace ClassLibrary2
{
    public class TestableOrderHandler : IOrderHandler
    {
        public void Handle(Order order)
        {
            this.Order = order;
            //Console.WriteLine(JsonConvert.SerializeObject(order, Formatting.Indented));
        }

        public Order Order { get; private set; }
    }
}