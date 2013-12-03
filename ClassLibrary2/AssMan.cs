using System;
using System.Collections.Generic;
using System.Threading;

namespace ClassLibrary2
{
    public class AssMan : IOrderHandler
    {
        private readonly IOrderHandler _orderHandler;
        private IDictionary<string, double> _costs = new Dictionary<string, double>();

        public AssMan(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
            _costs.Add("Beans on Toast", 12.99);
        }

        public bool Handle(Order order)
        {
            Thread.Sleep(1000);

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
            _orderHandler.Handle(order);

            return true;
        }
    }
}