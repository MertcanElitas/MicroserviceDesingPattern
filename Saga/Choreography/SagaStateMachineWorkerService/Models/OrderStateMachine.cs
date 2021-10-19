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
        public State OrderCreated { get; set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            //OrderCreatedRequestEvent tetiklendiğinde.
            //Db deki order id ile messagedan gelen orderId farklı ise yeni bir instance üret dedik.
            Event(() => OrderCreatedRequestEvent,
                       y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId)
                       .SelectId(context => Guid.NewGuid()));

            Initially(When(OrderCreatedRequestEvent)
                .Then(context =>
            {
                context.Instance.BuyerId = context.Data.BuyerId;
                context.Instance.OrderId = context.Data.OrderId;
                context.Instance.CreatedDate = DateTime.Now;

                context.Instance.CVV = context.Data.PaymentMessage.CVV;
                context.Instance.Expiration = context.Data.PaymentMessage.Expiration;
                context.Instance.TotalPrice = context.Data.PaymentMessage.TotalPrice;
            }).Then(context => Console.WriteLine($"OrderCreatedRequestEvent before :{context.Instance}"))
              .Publish(context => new OrderCreatedEvent(context.CorrelationId.Value) { OrderItems = context.Data.OrderItems })
              .TransitionTo(OrderCreated)
              .Then(context => Console.WriteLine($"OrderCreatedRequestEvent after :{context.Instance}")));
        }
    }
}
