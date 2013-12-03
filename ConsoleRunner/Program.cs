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
            var cwHandler    = new TestableOrderHandler();
            var cashierInner = new Cashier(cwHandler);
            var cashier      = new BlockingCollectionAsyncHandler(cashierInner);
            var assMan       = new BlockingCollectionAsyncHandler(new AssMan(cashier));
            var cooks        = new[]
            {
                new BlockingCollectionAsyncHandler(new Cook(assMan, 200)),
                new BlockingCollectionAsyncHandler(new Cook(assMan, 500)),
                new BlockingCollectionAsyncHandler(new Cook(assMan, 900))
            };
            var dispatcher = new RetryDispatcher(new BackPressureDispatcher(cooks, 5));

            var waiter = new Waiter(dispatcher);

            RunTest(cooks, assMan, cashier, waiter, cashierInner, 500000);
        }

        private static void RunTest(BlockingCollectionAsyncHandler[] cooks, BlockingCollectionAsyncHandler assMan, BlockingCollectionAsyncHandler cashier, Waiter waiter, Cashier cashierInner, int orderCount)
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
        }
    }
}
