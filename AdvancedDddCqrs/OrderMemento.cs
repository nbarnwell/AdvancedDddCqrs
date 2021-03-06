﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AdvancedDddCqrs
{
    public class OrderMemento : IDynamicWrapper
    {
        private dynamic _content;
        private OrderItem[] _items;

        public Guid Id
        {
            get { return _content.Id; }
            set { _content.Id = value; }
        }

        public bool IsPaid
        {
            get { return _content.IsPaid; }
            set { _content.IsPaid = value; }
        }

        public OrderItem[] Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public string ServerId
        {
            get { return _content.ServerId; }
            set { _content.ServerId = value; }
        }

        public int TableNumber
        {
            get { return _content.TableNumber; }
            set { _content.TableNumber = value; }
        }

        public bool DodgeyCustomer
        {
            get { return _content.DodgeyCustomer; }
            set { _content.DodgeyCustomer = value; }
        }

        public OrderMemento()
        {
            _content = new ExpandoObject();
        }

        public OrderMemento(dynamic content)
        {
            _content = content;
            Items = BuildItemsArray(_content);
        }

        public dynamic GetContent()
        {
            return _content;
        }

        private OrderItem[] BuildItemsArray(dynamic content)
        {
            JArray contentArray = content.Items;
            var orderItems = new List<OrderItem>();

            foreach (JToken item in contentArray)
            {
                var ingredientsUsed = item.Value<JArray>("IngredientsUsed")
                                          .Select(x => x.Value<string>())
                                          .ToArray();

                var orderItem = new OrderItem
                {
                    Cost            = item.Value<double>("Cost"),
                    Name            = item.Value<string>("Name"),
                    Quantity        = item.Value<uint>("Quantity"),
                    IngredientsUsed = ingredientsUsed
                };

                orderItems.Add(orderItem);
            }

            return orderItems.ToArray();
        }
    }

    public interface IDynamicWrapper
    {
        dynamic GetContent();
    }
}