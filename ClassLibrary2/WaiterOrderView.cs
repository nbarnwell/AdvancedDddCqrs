using System;

namespace ClassLibrary2
{
    public class WaiterOrderView : IDynamicWrapper
    {
        public string ServerId
        {
            get { return Content.ServerId; }
            set { Content.ServerId = value; }
        }

        public WaiterOrderView(dynamic order)
        {
            if (order == null) throw new ArgumentNullException("order");
            Content = order;
        }

        public WaiterOrderView()
        {
        }

        public dynamic Content { get; private set; }
    }
}