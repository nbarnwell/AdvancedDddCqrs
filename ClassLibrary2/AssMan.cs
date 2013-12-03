using System;
using System.Collections.Generic;
using System.Threading;
using ClassLibrary2.Messages;

namespace ClassLibrary2
{
    public class AssMan : IHandler<PriceFood>
    {
        private readonly ITopicDispatcher _dispatcher;

        private readonly IDictionary<string, double> _costs = new Dictionary<string, double>();

        public AssMan(ITopicDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _dispatcher = dispatcher;
            _costs.Add("Beans on Toast", 12.99);
        }

        public bool Handle(PriceFood message)
        {
            var order = message.Order;

            foreach (var item in order.Items)
            {
                double itemCost;
                if (_costs.TryGetValue(item.Name, out itemCost))
                {
                    item.Cost = itemCost;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("No price found for item {0}", item.Name));
                }
            }

            _dispatcher.Publish(new Priced(order, message.CorrelationId, message.MessageId));

            return true;
        }
    }
}