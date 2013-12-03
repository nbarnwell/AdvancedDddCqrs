using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary2;
using ClassLibrary2.Messages;
using Newtonsoft.Json;

namespace ConsoleRunner
{
    public class CorrelationPicker : IHandler<OrderTaken>
    {
        private readonly ITopicDispatcher _topicDispatcher;
        private bool finished;

        public CorrelationPicker(ITopicDispatcher topicDispatcher)
        {
            if (topicDispatcher == null) throw new ArgumentNullException("topicDispatcher");
            _topicDispatcher = topicDispatcher;
        }

        public bool Handle(OrderTaken message)
        {
            if (!finished)
            {
                _topicDispatcher.Subscribe(message.CorrelationId.ToString(), new Printer());
            }
            finished = true;
            return true;
        }
    }

    class Printer : IHandler<IMessage>
    {
        public bool Handle(IMessage message)
        {
            Console.WriteLine("{0}: {1}", message,JsonConvert.SerializeObject(message, Formatting.Indented));
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BackPressureTest();
        }

        private static void BackPressureTest()
        {
            var topicDispatcher = new TopicDispatcher();

            var cwHandler = new TestableOrderHandler();
            var cashierInner = new Cashier(topicDispatcher);
            var cashier = new ThreadBoundary<Priced>(cashierInner);
            var assMan = new ThreadBoundary<Cooked>(new AssMan(topicDispatcher));
            var cooks = new[]
            {
                new ThreadBoundary<OrderTaken>(new Cook(topicDispatcher, 2000)),
                new ThreadBoundary<OrderTaken>(new Cook(topicDispatcher, 5000)),
                new ThreadBoundary<OrderTaken>(new Cook(topicDispatcher, 9000))
            };
            var cookDispatcher = new TTLSettingHandler<OrderTaken>(
                new ThreadBoundary<OrderTaken>(
                    new RetryDispatcher<OrderTaken>(
                        new TTLFilteringHandler<OrderTaken>(
                            new BackPressureDispatcher<OrderTaken>(cooks, 5)))), 1);

            var waiter = new Waiter(topicDispatcher);

            topicDispatcher.Subscribe(typeof(Priced).FullName, cashier);
            topicDispatcher.Subscribe(typeof(OrderTaken).FullName, cookDispatcher);
            topicDispatcher.Subscribe(typeof(Cooked).FullName, assMan);

            topicDispatcher.Subscribe(typeof(OrderTaken).FullName, new CorrelationPicker(topicDispatcher));

            RunTest(cooks, assMan, cashier, waiter, cashierInner, 5000);
        }

        private static void RunTest(
            ThreadBoundary<OrderTaken>[] cooks,
            ThreadBoundary<Cooked> assMan,
            ThreadBoundary<Priced> cashier,
            Waiter waiter,
            Cashier cashierInner,
            int orderCount)
        {
            //Task.Factory.StartNew(
            //    () =>
            //    {
            //        while (true)
            //        {
            //            Console.WriteLine("Cook 1 Queue Length:    {0}", cooks[0].QueueLength);
            //            Console.WriteLine("Cook 2 Queue Length:    {0}", cooks[1].QueueLength);
            //            Console.WriteLine("Cook 3 Queue Length:    {0}", cooks[2].QueueLength);
            //            Console.WriteLine("AssMan Queue Length:    {0}", assMan.QueueLength);
            //            Console.WriteLine("Cashier Queue Length:   {0}", cashier.QueueLength);
            //            Console.WriteLine("-");
            //            Thread.Sleep(500);
            //        }
            //    },
            //    TaskCreationOptions.AttachedToParent);

            var orderIds = new BlockingCollection<Guid>();
            var ordersToBePaid = new BlockingCollection<Guid>();

            for (int i = 0; i < orderCount; i++)
            {
                orderIds.Add(Guid.NewGuid());
            }

            Task.Factory.StartNew(
                () =>
                {
                    foreach (var orderId in orderIds)
                    {
                        waiter.TakeOrder(
                            12,
                            new[]
                            {
                                new OrderItem
                                {
                                    Name = "Beans on Toast",
                                    Quantity = 1
                                }
                            },
                            orderId
                            );

                        ordersToBePaid.Add(orderId);
                    }

                    ordersToBePaid.CompleteAdding();
                });

            var waitHandle = new ManualResetEvent(false);
            Task.Factory.StartNew(
                () =>
                {
                    foreach (var orderId in ordersToBePaid.GetConsumingEnumerable())
                    {
                        while (cashierInner.TryPay(orderId) == false)
                        {
                            Thread.Sleep(1);
                        }
                    }

                    waitHandle.Set();
                });

            waitHandle.WaitOne();
            Console.ReadKey();
        }
    }
}
