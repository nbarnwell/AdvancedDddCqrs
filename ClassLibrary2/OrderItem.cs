using System.Collections.Generic;

namespace ClassLibrary2
{
    public class OrderItem
    {
        public string Name { get; set; }
        public uint Quantity { get; set; }
        public IList<string> IngredientsUsed { get; set; }
        public double Cost { get; set; }
    }
}