using MassTransit;
using Shared;
using Shared.Events;
using Shared.Interfaces;

namespace Payment.Api.Consumers
{
    public class StockReservedRequestPaymentEventConsumer(IPublishEndpoint publishEndpoint, ILogger<StockReservedRequestPaymentEventConsumer> logger) : IConsumer<IStockReservedRequestPaymentEvent>
    {
        public async Task Consume(ConsumeContext<IStockReservedRequestPaymentEvent> context)
        {
            var balance = 300m;

            if (balance > context.Message.Payment.TotalPrice)
            {
                logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was withdrawn from cretid card for buyer Id: {context.Message.BuyerId}");

                await publishEndpoint.Publish(new PaymentCompletedEvent(context.Message.CorrelationId));
            }
            else
            {
                logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was not withdrawn from credit card for buyer Id : {context.Message.BuyerId}");

                await publishEndpoint.Publish(new PaymentFailEvent(context.Message.CorrelationId, context.Message.BuyerId, context.Message.OrderItems,
                    "not enough balance"));
            }
        }
    }
}
