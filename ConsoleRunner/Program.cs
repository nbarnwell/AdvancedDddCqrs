using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using AdvancedDddCqrs;

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
            var tbm = new ThreadBoundaryMonitor();

            var cashierInner = new Cashier(topicDispatcher);
            var cashier      = tbm.Wrap(cashierInner);
            var assMan       = tbm.Wrap(new AssMan(topicDispatcher));
            var cooks        = new[]
            {
                tbm.Wrap(new Cook(topicDispatcher, 2000)),
                tbm.Wrap(new Cook(topicDispatcher, 5000)),
                tbm.Wrap(new Cook(topicDispatcher, 9000))
            };
            var cookDispatcher =
                TTLSettingHandler.Wrap(
                    tbm.Wrap(
                        RetryDispatcher.Wrap(
                            TTLFilteringHandler.Wrap(
                                SmartDispatcher.Wrap(cooks, maxQueueLength: 5))
                            )
                        ),
                    1);

            var waiter = new Waiter("Neil", topicDispatcher);

            topicDispatcher.Subscribe(cashier);
            topicDispatcher.Subscribe(cookDispatcher);
            topicDispatcher.Subscribe(assMan);

            //topicDispatcher.Subscribe(new SelfUnsubscribingCorrelationPicker(topicDispatcher));
            topicDispatcher.Subscribe(new OrderSampler(topicDispatcher));

            topicDispatcher.Subscribe(tbm.Wrap(new OrderFulfillmentCoordinator(topicDispatcher)));

            RunTest(waiter, cashierInner, orderCount: 1);
        }


        private static void RunTest(Waiter waiter, Cashier cashier, int orderCount)
        {
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
                        while (cashier.TryPay(orderId) == false)
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
