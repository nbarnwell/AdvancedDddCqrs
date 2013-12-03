using System;
using System.Collections.Generic;
using System.Threading;

namespace ClassLibrary2
{
    public class Cook : IOrderHandler
    {
        private readonly IOrderHandler _orderHandler;
        private readonly IDictionary<string, IList<string>> _recipes = new Dictionary<string, IList<string>>();
        private readonly int _sleepDuration;

        public Cook(IOrderHandler orderHandler, int sleepDuration)
        {
            if (orderHandler == null) throw new ArgumentNullException("orderHandler");
            _orderHandler = orderHandler;
            _sleepDuration = sleepDuration;
            _recipes.Add("Beans on Toast", new[] { "Beans", "Toast" });
        }

        public bool Handle(Order order)
        {
            Thread.Sleep(_sleepDuration);

            foreach (var item in order.Items)
            {
                IList<string> ingredients;
                if (_recipes.TryGetValue(item.Name, out ingredients))
                {
                    item.IngredientsUsed = ingredients;
                }
                else
                {
                    throw new Exception(string.Format("Cook doesn't know how to make {0}", item.Name));
                }
            }

            _orderHandler.Handle(order);

            return true;
        }
    }
}