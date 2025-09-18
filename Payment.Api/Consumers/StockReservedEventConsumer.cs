using MassTransit;
using Shared;

namespace Payment.Api.Consumers
{
    public class StockReservedEventConsumer(IPublishEndpoint publishEndpoint,ILogger<StockReservedEventConsumer> logger) : IConsumer<StockReservedEvent>
    {
        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var balance = 300m;

            if (balance > context.Message.Payment.TotalPrice)
            {
                logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was withdrawn from cretid card for user Id: {context.Message.BuyerId}");

                await publishEndpoint.Publish(new PaymentCompletedEvent(context.Message.OrderId,
                    context.Message.BuyerId));
            }
            else
            {
                logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was not withdrawn from credit card for user ID : {context.Message.BuyerId}");

                await publishEndpoint.Publish(new PaymentFailEvent(context.Message.OrderId, context.Message.BuyerId,context.Message.OrderItems,
                    "not enough balance"));
            }
        }
    }
}
