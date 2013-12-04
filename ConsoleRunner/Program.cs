using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
                tbm.Wrap(new Cook(topicDispatcher, 20)),
                tbm.Wrap(new Cook(topicDispatcher, 50)),
                tbm.Wrap(new Cook(topicDispatcher, 90))
            };
            var cookDispatcher =
                TTLSettingHandler.Wrap(
                    tbm.Wrap(
                        RetryDispatcher.Wrap(
                            TTLFilteringHandler.Wrap(
                                SmartDispatcher.Wrap(cooks, maxQueueLength: 15))
                            )
                        ),
                    10);

            var waiter = new Waiter("Neil", topicDispatcher);

            topicDispatcher.Subscribe(cashier);
            topicDispatcher.Subscribe(cookDispatcher);
            topicDispatcher.Subscribe(assMan);
            topicDispatcher.Subscribe(reporting);
            topicDispatcher.Subscribe(tbm.Wrap(new Logger()));

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
                    bool isDodegy = false;
                       
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
                            orderId,
                            isDodegy
                            );
                        isDodegy = !isDodegy;
                        ordersToBePaid.Add(orderId);
                    }

                    //ordersToBePaid.CompleteAdding();
                });

            var waitHandle = new ManualResetEvent(false);
            Task.Factory.StartNew(
                () =>
                {
                    foreach (var orderId in ordersToBePaid.GetConsumingEnumerable())
                    {
                        if (false == cashier.TryPay(orderId))
                        {
                            ordersToBePaid.Add(orderId);
                        }
                    }

                    waitHandle.Set();
                });

            waitHandle.WaitOne();
            Console.ReadKey();
        }
    }
}
