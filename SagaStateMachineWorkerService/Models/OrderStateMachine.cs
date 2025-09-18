using MassTransit;
using Shared;
using Shared.Events;
using Shared.Interfaces;
using Shared.Messages;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }
        public Event<IStockNotReservedEvent> StockNotReservedEvent { get; set; }
        public Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<IPaymentFailEvent> PaymentFaiEvent { get; set; }


        public State OrderCreated { get;  }
        public State StockReserved { get; }
        public State StockNotReserved { get; }
        public State PaymentCompleted { get; }
        public State PaymentFailed { get; }


        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderCreatedRequestEvent, y
                => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId)
                    .SelectId(_ => Guid.NewGuid()));

            Event(() => StockReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            Event(() => StockNotReservedEvent, x => x.CorrelateById(y=> y.Message.CorrelationId));

            Event(() => PaymentCompletedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));

            Event(() => PaymentFaiEvent, x => x.CorrelateById(y => y.Message.CorrelationId));



            Initially(When(OrderCreatedRequestEvent).Then(context =>
            {
                context.Saga.BuyerId = context.Message.BuyerId;
                context.Saga.OrderId = context.Message.OrderId;
                context.Saga.TotalPrice = context.Message.Payment.TotalPrice;
                context.Saga.CreatedDate = DateTime.Now;

                context.Saga.CardName = context.Message.Payment.CardName;
                context.Saga.CardNumber = context.Message.Payment.CardNumber;
                context.Saga.Cvv = context.Message.Payment.Cvv;
                context.Saga.Exp = context.Message.Payment.Exp;
            }).Then(context =>
            {
                Console.WriteLine($"OrderCreatedRequestEvent Before : {context.Saga}");
            })
            .Publish(context => new OrderCreatedEvent(context.Message.OrderItems,context.Saga.CorrelationId))
            .TransitionTo(OrderCreated).Then(context =>
            {
                Console.WriteLine($"OrderCreatedRequestEvent After : {context.Saga}");
            }));

            During(OrderCreated,
                When(StockReservedEvent)
                    .TransitionTo(StockReserved)
                    .Send(new Uri($"queue:{RabbitMqSettingsConst.PaymentStockReservedRequestQueueName}"),
                        context => new StockReservedRequestPaymentEvent(
                            new PaymentMessage()
                            {
                                CardName = context.Saga.CardName,
                                TotalPrice = context.Saga.TotalPrice,
                                CardNumber = context.Saga.CardNumber,
                                Cvv = context.Saga.Cvv,
                                Exp = context.Saga.Exp
                            },
                            context.Message.OrderItems,
                            context.Saga.CorrelationId,
                            context.Saga.BuyerId
                        ))
                    .Then(context => { Console.WriteLine($"StockReserved After : {context.Saga}"); }),
                When(StockNotReservedEvent).TransitionTo(StockNotReserved)
                    .Publish(context => new OrderRequestFailedEvent(context.Saga.OrderId,context.Message.FailMessage))
                    .Then(context => { Console.WriteLine($"StockNotReserved After : {context.Saga}"); }));


            During(StockReserved, When(PaymentCompletedEvent).TransitionTo(PaymentCompleted)
                .Publish(context => new OrderRequestCompletedEvent(context.Saga.OrderId))
                .Then(context => {Console.WriteLine($"PaymentCompleted After : {context.Saga}");})
                .Finalize(),
                When(PaymentFaiEvent)
                    .Publish(context => new OrderRequestFailedEvent(context.Saga.OrderId,context.Message.FailMessage))
                    .Send(new Uri($"queue:{RabbitMqSettingsConst.StockRollBackMessageQueueName}"),context => new StockRollBackMessage(context.Message.OrderItems,context.Saga.OrderId))
                    .TransitionTo(PaymentFailed)
                );

            SetCompletedWhenFinalized();

        }
    }
}
