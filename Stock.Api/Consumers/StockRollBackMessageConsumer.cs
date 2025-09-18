using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;
using Stock.Api.Models;

namespace Stock.Api.Consumers
{
    public class StockRollBackMessageConsumer(AppDbContext appDbContext, ILogger<StockRollBackMessageConsumer> logger) : IConsumer<IStockRollBackMessage>
    {
        public async Task Consume(ConsumeContext<IStockRollBackMessage> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                var stock = await appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock is not null)
                {
                    stock.Count += item.Count;
                    await appDbContext.SaveChangesAsync();
                }
            }

            logger.LogInformation($"Stock was released for order id : {context.Message.OrderId}");
        }
    }
}
