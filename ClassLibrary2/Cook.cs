using System;
using System.Collections.Generic;
using System.Threading;

namespace ClassLibrary2
{
    public class Cook : IOrderHandler
    {
        private readonly IOrderHandler _orderHandler;
        private IDictionary<string, IList<string>> _recipes = new Dictionary<string, IList<string>>();

        public Cook(IOrderHandler orderHandler)
        {
            if (orderHandler == null) throw new ArgumentNullException("orderHandler");
            _orderHandler = orderHandler;

            _recipes.Add("Beans on Toast", new[] { "Beans", "Toast" });
        }

        public void Handle(Order order)
        {
            Thread.Sleep(1000);

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
        }
    }
}