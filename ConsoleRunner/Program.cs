using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using AdvancedDddCqrs;
using AdvancedDddCqrs.Messages;

namespace ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            BackPressureTest();
        }

        private static void BackPressureTest()
        {
            var topicDispatcher = new TopicDispatcher();

            var cashierInner = new Cashier(topicDispatcher);
            var cashier = new ThreadBoundary<RegisterOrder>(cashierInner);
            var assMan = new ThreadBoundary<PriceFood>(new AssMan(topicDispatcher));
            var cooks = new[]
            {
                new ThreadBoundary<CookFood>(new Cook(topicDispatcher, 2000)),
                new ThreadBoundary<CookFood>(new Cook(topicDispatcher, 5000)),
                new ThreadBoundary<CookFood>(new Cook(topicDispatcher, 9000))
            };
            var cookDispatcher =
                new TTLSettingHandler<CookFood>(
                    new ThreadBoundary<CookFood>(
                        new RetryDispatcher<CookFood>(
                            new TTLFilteringHandler<CookFood>(
                                new BackPressureDispatcher<CookFood>(cooks, 5)))), 1);

            var waiter = new Waiter(topicDispatcher);

            topicDispatcher.Subscribe(typeof(RegisterOrder).FullName, cashier);
            topicDispatcher.Subscribe(typeof(CookFood).FullName, cookDispatcher);
            topicDispatcher.Subscribe(typeof(PriceFood).FullName, assMan);

            topicDispatcher.Subscribe(typeof(OrderTaken).FullName, new CorrelationPicker(topicDispatcher));

            topicDispatcher.Subscribe(typeof(OrderTaken).FullName, new Coordinator(topicDispatcher));

            RunTest(cooks, assMan, cashier, waiter, cashierInner, 5000);
        }

        private static void RunTest(
            ThreadBoundary<CookFood>[] cooks,
            ThreadBoundary<PriceFood> assMan,
            ThreadBoundary<RegisterOrder> cashier,
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
