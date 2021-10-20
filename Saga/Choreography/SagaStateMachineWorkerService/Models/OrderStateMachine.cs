using Automatonymous;
using Common;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }
        public Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<IPaymentFailedEvent> PaymentFailedEvent { get; set; }
        public Event<IStockNotReserverdEvent> StockNotReserverdEvent { get; set; }

        public State OrderCreated { get; private set; }
        public State StockReserved { get; private set; }
        public State StockNotReserverd { get; private set; }
        public State PaymentCompleted { get; private set; }
        public State PaymentFailed { get; private set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            //OrderCreatedRequestEvent tetiklendiğinde.
            //Db deki order id ile messagedan gelen orderId farklı ise yeni bir instance üret dedik.
            Event(() => OrderCreatedRequestEvent,
                       y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId)
                       .SelectId(context => Guid.NewGuid()));

            //Todo buraya çalışılıcak.
            Event(() => StockReservedEvent, x => x.CorrelateById(a => a.Message.CorrelationId));
            Event(() => StockNotReserverdEvent, x => x.CorrelateById(a => a.Message.CorrelationId));
            Event(() => PaymentCompletedEvent, x => x.CorrelateById(a => a.Message.CorrelationId));

            Initially(When(OrderCreatedRequestEvent)
                .Then(context =>
                {
                    context.Instance.BuyerId = context.Data.BuyerId;
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.CreatedDate = DateTime.Now;
               
                    context.Instance.CVV = context.Data.PaymentMessage.CVV;
                    context.Instance.Expiration = context.Data.PaymentMessage.Expiration;
                    context.Instance.TotalPrice = context.Data.PaymentMessage.TotalPrice;
                })
               .Then(context => Console.WriteLine($"OrderCreatedRequestEvent before :{context.Instance}"))
               .Publish(context => new OrderCreatedEvent(context.CorrelationId.Value) { OrderItems = context.Data.OrderItems })
               .TransitionTo(OrderCreated)
               .Then(context => Console.WriteLine($"OrderCreatedRequestEvent after :{context.Instance}")));


            During(OrderCreated,
                When(StockReservedEvent)
                  .TransitionTo(StockReserved)
                  .Send(new Uri($"queue:{RabbitMQConstants.PaymentStockReservedRequestQueueName}"),
                        context => new StockReservedPaymentRequest(context.Instance.CorrelationId)
                        {
                            OrderItemMessages = context.Data.OrderItemMessages,
                            BuyerId = context.Instance.BuyerId,
                            Payment = new PaymentMessage()
                            {
                                CardName = context.Instance.CardName,
                                CardNumber = context.Instance.CardNumber,
                                CVV = context.Instance.CVV,
                                Expiration = context.Instance.Expiration,
                                TotalPrice = context.Instance.TotalPrice
                            }
                        })
                  .Then(context => { Console.WriteLine($"StockReservedEvent After:{context.Instance}"); }),
                When(StockNotReserverdEvent)
                  .TransitionTo(StockNotReserverd)
                  .Publish(context => new OrderRequestFailedEvent(context.Data.CorrelationId)
                  {
                      OrderId = context.Instance.OrderId,
                      Reason = context.Data.Message
                  }));

            During(StockReserved,
                 When(PaymentCompletedEvent)
                   .TransitionTo(PaymentCompleted)
                   .Publish(context => new OrderRequestCompletedEvent(context.CorrelationId.Value) { OrderId = context.Instance.OrderId })
                   .Then(context => { Console.WriteLine($"OrderRequestCompletedEvent After:{context.Instance}"); }).Finalize(),
                 When(PaymentFailedEvent)
                   .Publish(context => new OrderRequestFailedEvent(context.Data.CorrelationId)
                   {
                       OrderId = context.Instance.OrderId,
                       Reason = context.Data.Reason
                   })
                   .Send(new Uri($"queue:{RabbitMQConstants.StockRollBackMessageQueueName}"), context => new StockRollbackMessage()
                   {
                       OrderItems = context.Data.OrderItemMessages
                   })
                   .TransitionTo(PaymentFailed)
                 );

            SetCompletedWhenFinalized();
        }
    }
}
