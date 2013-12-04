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
            var tbm = new ThreadBoundaryMonitor();
            
            var reporting = tbm.Wrap(new ReportingSystem(topicDispatcher));
            var cashierInner = new Cashier(topicDispatcher);
            var cashier      = tbm.Wrap(cashierInner);
            var assMan       = tbm.Wrap(new AssMan(topicDispatcher));
            var cooks        = new[]
            {
                tbm.Wrap(new Cook(topicDispatcher, 200)),
                tbm.Wrap(new Cook(topicDispatcher, 500)),
                tbm.Wrap(new Cook(topicDispatcher, 900))
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
            topicDispatcher.Subscribe(reporting);

            topicDispatcher.Subscribe(new SelfUnsubscribingOrderSampler(topicDispatcher));

            topicDispatcher.Subscribe(tbm.Wrap<OrderTaken>(new OrderFulfillmentCoordinator(topicDispatcher)));
            tbm.Start();

            RunTest(waiter, cashierInner, orderCount: 5000);
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
