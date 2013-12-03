using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary2;

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

            var cwHandler    = new TestableOrderHandler();
            var cashierInner = new Cashier(topicDispatcher);
            var cashier      = new ThreadBoundary(cashierInner);
            var assMan = new ThreadBoundary(new AssMan(topicDispatcher));
            var cooks        = new[]
            {
                new ThreadBoundary(new Cook(topicDispatcher, 2000)),
                new ThreadBoundary(new Cook(topicDispatcher, 5000)),
                new ThreadBoundary(new Cook(topicDispatcher, 9000))
            };
            var dispatcher = new TTLSettingHandler(
                new ThreadBoundary(
                    new RetryDispatcher(
                        new TTLFilteringHandler(
                            new BackPressureDispatcher(cooks, 5)))), 1);

            var waiter = new Waiter(topicDispatcher);

            RunTest(cooks, assMan, cashier, waiter, cashierInner, 5000);
        }

        private static void RunTest(ThreadBoundary[] cooks, ThreadBoundary assMan, ThreadBoundary cashier, Waiter waiter, Cashier cashierInner, int orderCount)
        {
            Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        Console.WriteLine("Cook 1 Queue Length:    {0}", cooks[0].QueueLength);
                        Console.WriteLine("Cook 2 Queue Length:    {0}", cooks[1].QueueLength);
                        Console.WriteLine("Cook 3 Queue Length:    {0}", cooks[2].QueueLength);
                        Console.WriteLine("AssMan Queue Length:    {0}", assMan.QueueLength);
                        Console.WriteLine("Cashier Queue Length:   {0}", cashier.QueueLength);
                        Console.WriteLine("-");
                        Thread.Sleep(500);
                    }
                },
                TaskCreationOptions.AttachedToParent);

            var orderIds = new BlockingCollection<Guid>();

            Task.Factory.StartNew(
                () =>
                {
                    for (int i = 0; i < orderCount; i++)
                    {
                        var orderId = Guid.NewGuid();
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

                        orderIds.Add(orderId);
                    }

                    orderIds.CompleteAdding();
                });

            var waitHandle = new ManualResetEvent(false);
            Task.Factory.StartNew(
                () =>
                {
                    foreach (var orderId in orderIds.GetConsumingEnumerable())
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
