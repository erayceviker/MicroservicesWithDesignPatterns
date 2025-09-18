using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.Api.Models;

namespace Stock.Api.Consumers
{
    public class OrderCreatedEventConsumer(AppDbContext appDbContext,ILogger<OrderCreatedEventConsumer> logger,
        ISendEndpointProvider sendEndpointProvider,IPublishEndpoint publishEndpoint) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();

            foreach (var item in context.Message.OrderItems)
            {
                stockResult.Add(await appDbContext.Stocks.AnyAsync(x=> x.ProductId == item.ProductId && x.Count > item.Count));
            }

            if (stockResult.All(x => x.Equals(true)))
            {

                foreach (var item in context.Message.OrderItems)
                {
                    var stock = await appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                    if (stock is not null)
                    {
                        stock.Count -= item.Count;
                    }

                    await appDbContext.SaveChangesAsync();
                }

                logger.LogInformation($"Stock was reserved for buyer Id: {context.Message.BuyerId}");

                var sendEndPoint =
                    await sendEndpointProvider.GetSendEndpoint(
                        new Uri($"queue:{RabbitMqSettingsConst.StockReservedEventQueueName}"));

                var stockReservedEvent = new StockReservedEvent(context.Message.OrderId, context.Message.BuyerId,
                    context.Message.Payment, context.Message.OrderItems);

                await sendEndPoint.Send(stockReservedEvent);
            }
            else
            {
                await publishEndpoint.Publish(new StockNotReservedEvent(context.Message.OrderId, "Not enough stock"));
                logger.LogInformation($"not enough stock for buyer Id : {context.Message.BuyerId}");
            }
        }
    }
}
