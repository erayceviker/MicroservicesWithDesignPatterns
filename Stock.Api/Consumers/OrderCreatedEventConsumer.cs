using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Shared.Interfaces;
using Stock.Api.Models;

namespace Stock.Api.Consumers
{
    public class OrderCreatedEventConsumer(AppDbContext appDbContext, ILogger<OrderCreatedEventConsumer> logger, 
        IPublishEndpoint publishEndpoint) : IConsumer<IOrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();

            foreach (var item in context.Message.OrderItems)
            {
                stockResult.Add(await appDbContext.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count > item.Count));
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

                logger.LogInformation($"Stock was reserved for CorrelationId: {context.Message.CorrelationId}");

                var stockReservedEvent = new StockReservedEvent(context.Message.OrderItems,context.Message.CorrelationId);

                await publishEndpoint.Publish(stockReservedEvent);
            }
            else
            {
                await publishEndpoint.Publish(new StockNotReservedEvent("Not enough stock",context.Message.CorrelationId));
                logger.LogInformation($"not enough stock for CorrelationId : {context.Message.CorrelationId}");
            }
        }
    }
}
