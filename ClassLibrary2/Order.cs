using System;
using System.Collections.Generic;

namespace ClassLibrary2
{
    public class Order
    {
        public int TableNumber { get; private set; }
        public IList<OrderItem> Items { get; set; }

        public Guid Id { get; set; }

        public bool IsPaid { get; set; }

        public Order(int tableNumber, Guid id)
        {
            Items = new List<OrderItem>();
            Id = id;
            TableNumber = tableNumber;
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
        }
    }
}