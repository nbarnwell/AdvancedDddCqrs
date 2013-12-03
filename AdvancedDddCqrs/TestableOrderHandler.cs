using System;
using Newtonsoft.Json;

namespace AdvancedDddCqrs
{
    public class TestableOrderHandler : IOrderHandler
    {
        public bool Handle(Order order)
        {
            this.Order = order;
            ////Console.WriteLine(JsonConvert.SerializeObject(order, Formatting.Indented));

            return true;
        }

        public Order Order { get; private set; }
    }
}