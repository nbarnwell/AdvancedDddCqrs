using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AdvancedDddCqrs
{
    public class Order : ISupportMemoisation<OrderMemento>
    {
        private readonly List<OrderItem> _items;

        public int    TableNumber { get; private set; }
        public Guid   Id          { get; private set; }
        public string ServerId    { get; private set; }
        public bool   IsPaid      { get; private set; }
        
        public IEnumerable<OrderItem> Items
        {
            get { return _items; }
        }

        public Order(OrderMemento memento)
        {
            TableNumber = memento.TableNumber;
            Id          = memento.Id;
            ServerId    = memento.ServerId;
            IsPaid      = memento.IsPaid;
            _items      = new List<OrderItem>(memento.Items);
        }

        public Order(int tableNumber, Guid id, string serverId)
        {
            Id          = id;
            TableNumber = tableNumber;
            ServerId    = serverId;
            _items      = new List<OrderItem>();
        }

        public OrderMemento GetMemento()
        {
            return new OrderMemento
            {
                Id          = Id,
                IsPaid      = IsPaid,
                ServerId    = ServerId,
                TableNumber = TableNumber,
                Items       = Items.Select(x => new OrderItem
                {
                    Cost            = x.Cost,
                    Name            = x.Name,
                    Quantity        = x.Quantity,
                    IngredientsUsed = x.IngredientsUsed.ToList()
                }).ToArray()
            };
        }

        public Order Clone()
        {
            return new Order(GetMemento());
        }

        public void AddItem(OrderItem item)
        {
            _items.Add(item);
        }

        public void SettleBill()
        {
            IsPaid = true;
        }

        public void MoveTable(int newTableNumber)
        {
            TableNumber = newTableNumber;
        }
    }
}