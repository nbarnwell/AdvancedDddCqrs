using System.Collections.Generic;
using System.Linq;

namespace AdvancedDddCqrs
{
    public class OrderItem
    {
        public string Name { get; set; }
        public uint Quantity { get; set; }
        public IList<string> IngredientsUsed { get; set; }
        public double Cost { get; set; }

        public OrderItem Clone()
        {
            return new OrderItem
            {
                Name            = Name,
                Quantity        = Quantity,
                IngredientsUsed = IngredientsUsed.ToList(),
                Cost            = Cost
            };
        }
    }
}